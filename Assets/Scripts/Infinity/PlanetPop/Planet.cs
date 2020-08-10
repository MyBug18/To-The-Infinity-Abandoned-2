using Infinity.HexTileMap;
using Infinity.Modifiers;
using Infinity.PlanetPop.BuildingCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Infinity.GameData;

namespace Infinity.PlanetPop
{
    /// <summary>
    /// Inhabitable planet
    /// </summary>
    public class Planet : IPlanet, IModifierHolder, ITileMapHolder
    {
        public string Name { get; private set; }

        public HexTileCoord HexCoord { get; private set; }

        public string PlanetType { get; }

        public readonly int Size;

        #region ITileMapHolder

        private readonly TileMap _tileMap;

        public int TileMapRadius => _tileMap.Radius;

        public TileMapType TileMapType => TileMapType.Planet;

        public IReadOnlyList<IOnHexTileObject> this[HexTileCoord coord] => _tileMap[coord];

        #endregion ITileMapHolder

        #region ISignalDispatcher

        private readonly Neuron _neuron;

        Type ISignalDispatcherHolder.HolderType => typeof(Planet);

        public SignalDispatcher SignalDispatcher { get; }

        #endregion ISignalDispatcher

        #region Pop

        private readonly List<Pop> _pops = new List<Pop>();

        private readonly List<Pop> _unemployedPops = new List<Pop>();

        public IReadOnlyList<Pop> Pops => _pops;

        public IReadOnlyList<Pop> UnemployedPops => _unemployedPops;

        public const float InitialPopGrowth = 5f;

        private readonly Dictionary<string, int> _popGrowthMultipliers = new Dictionary<string, int>();

        public float CurrentPopGrowth { get; private set; }

        private readonly List<(Pop pop, PopSlot slot, int RemainTurn)> _trainingCenter =
            new List<(Pop pop, PopSlot slot, int RemainTurn)>();

        public IReadOnlyList<(Pop pop, PopSlot slot, int RemainTurn)> TrainingCenter => _trainingCenter;

        #endregion Pop

        #region Resources

        private readonly Dictionary<string, float> _currentResourceKeep;

        public IReadOnlyDictionary<string, float> CurrentResourceKeep => _currentResourceKeep;

        public IReadOnlyDictionary<string, float> YieldFromJob => _yieldFromJobCache;

        private Dictionary<string, float> _yieldFromJobCache = new Dictionary<string, float>();

        public IReadOnlyDictionary<string, float> UpkeepFromJob => _upkeepFromJobCache;

        private Dictionary<string, float> _upkeepFromJobCache = new Dictionary<string, float>();

        #endregion Resources

        public IReadOnlyList<Building> Buildings => GetTileObjectList<Building>();

        public readonly PlanetBuildingFactory BuildingFactory;

        private readonly List<Modifier> _modifiers = new List<Modifier>();

        public IReadOnlyList<Modifier> Modifiers => _modifiers;

        public Planet(Neuron parentNeuron, string name, HexTileCoord coord, int size)
        {
            _neuron = parentNeuron.GetChildNeuron(this);
            SignalDispatcher = new SignalDispatcher(_neuron);

            BuildingFactory = new PlanetBuildingFactory(_neuron, this);

            _neuron.Subscribe<PopToTrainingCenterSignal>(OnPopToTrainingCenterSignal);
            _neuron.Subscribe<BuildingQueueEndedSignal>(OnBuildingQueueEndedSignal);
            _neuron.Subscribe<GameCommandSignal>(OnGameCommandSignal);
            _neuron.Subscribe<ModifierSignal>(OnModifierSignal);

            _neuron.EventConditionPasser.SetRefiner(OnPassiveEventCheck);

            HexCoord = coord;
            Name = name;
            Size = size;
            PlanetType = "Inhabitable";

            InitializeResourceKeep();

            _tileMap = new TileMap(6, _neuron);
        }

        /// <summary>
        /// Just for test
        /// </summary>
        public Planet()
        {
            _tileMap = new TileMap(6, null);
        }

        private void InitializeResourceKeep()
        {
            foreach (var kv in GameDataStorage.Instance.GetGameData<GameFactorData>().PlanetaryResourceDict)
            {
                if (kv.Value)
                    _currentResourceKeep.Add(kv.Key, 0);
            }
        }

        private void AddRemoveModifier(Modifier m, bool isAdding)
        {
            _neuron.SendSignal(new ModifierSignal(this, m, isAdding), SignalDirection.Downward);
        }

        private void OnModifierSignal(ISignal s)
        {
            if (!(s is ModifierSignal ms)) return;

            var m = ms.Modifier;

            if (ms.IsForTile) return;

            if (ms.IsAdding)
            {
                _modifiers.Add(m);
            }
            else
            {
                _modifiers.Remove(m);
            }


            ApplyModifierChange(m, ms.IsAdding);
        }

        private void ApplyModifierChange(Modifier m, bool IsAdding)
        {
            var resourceData = GameDataStorage.Instance.GetGameData<GameFactorData>();

            foreach (var kv in m.ModifierInfo.GameFactorAmount)
            {
                if (resourceData.GetFactorKind(kv.Key) == GameFactorKind.PlanetaryFactor)
                {

                }
            }
        }

        private Dictionary<string, float> GetBuildingYield()
        {
            var result = new Dictionary<string, float>();

            foreach (var b in Buildings)
            {
                foreach (var kv in b.YieldFromJob)
                {
                    if (!result.ContainsKey(kv.Key))
                        result.Add(kv.Key, 0);

                    result[kv.Key] += kv.Value;
                }
            }

            return result;
        }

        private Dictionary<string, float> GetBuildingUpkeep()
        {
            var result = new Dictionary<string, float>();

            foreach (var b in Buildings)
            {
                foreach (var kv in b.UpkeepFromJob)
                {
                    if (!result.ContainsKey(kv.Key))
                        result.Add(kv.Key, 0);

                    result[kv.Key] += kv.Value;
                }
            }

            return result;
        }

        private void RecalculateJobResources()
        {
            _yieldFromJobCache = GetBuildingYield();
            _upkeepFromJobCache = GetBuildingUpkeep();
        }

        private void OnGameCommandSignal(ISignal s)
        {
            if (!(s is GameCommandSignal gcs) || gcs.CommandType != GameCommandType.StartNewTurn) return;

            ApplyPopGrowth();
            ProceedTraining();
            ApplyTurnResource();
        }

        /// <summary>
        /// Planetary resources only
        /// </summary>
        private void ApplyTurnResource()
        {
        }

        private void ApplyPopGrowth()
        {
            CurrentPopGrowth += InitialPopGrowth * (_popGrowthMultipliers.Values.Sum() / 100f + 1);

            if (CurrentPopGrowth < 100) return;

            var newPop = new Pop(_neuron, "TestPop", new HexTileCoord(_tileMap.Radius, _tileMap.Radius));
            _neuron.SendSignal(new PopBirthSignal(this, newPop), SignalDirection.Upward);
            _unemployedPops.Add(newPop);
        }

        private void ProceedTraining()
        {
            var removeIdx = new List<int>();

            for (var i = 0; i < _trainingCenter.Count; i++)
            {
                var (pop, slot, remainTurn) = _trainingCenter[i];
                _trainingCenter[i] = (pop, slot, remainTurn - 1);

                if (_trainingCenter[i].RemainTurn != 0) continue;

                removeIdx.Add(i);

                _neuron.SendSignal(new PopSlotAssignedSignal(this, pop, slot), SignalDirection.Downward);
            }

            for (var i = removeIdx.Count - 1; i >= 0; i--)
                _trainingCenter.RemoveAt(i);
        }

        private void OnBuildingQueueEndedSignal(ISignal s)
        {
            if (!(s is BuildingQueueEndedSignal bqes)) return;

            var coord = bqes.QueueElement.Coord;

            var modifiers = _modifiers.ToList();
            modifiers.AddRange(GetHexTile(coord).Modifiers);

            var building = new Building(_neuron, bqes.QueueElement.Prototype, this, coord, modifiers);

            _tileMap.AddTileObject(coord, building);

            _neuron.SendSignal(new BuildingConstructedSignal(this, building.Name, coord), SignalDirection.Downward);
        }

        private void OnPopToTrainingCenterSignal(ISignal s)
        {
            if (!(s is PopToTrainingCenterSignal pttcs)) return;

            var trainingTime = GameDataStorage.Instance.GetGameData<PopSlotData>()
                .GetTrainingTime(pttcs.Pop.Aptitude, pttcs.DestinationSlot.Name);

            _trainingCenter.Add((pttcs.Pop, pttcs.DestinationSlot, trainingTime));
        }

        private List<PassiveEventPrototype> OnPassiveEventCheck(List<PassiveEventPrototype> events)
        {
            var passed = new List<PassiveEventPrototype>();

            foreach (var prototype in events.Where(prototype =>
                prototype.PlanetConditionChecker?.Invoke(this) ?? true))
            {
                if (prototype.EventOwnerType == "Planet" && Utils.GetBoolFromChance(prototype.Chance))
                {
                    // occur event and send signal
                }
                else
                {
                    passed.Add(prototype);
                }
            }

            return passed;
        }

        PlanetStatus IPlanet.GetPlanetStatus() => _pops.Count > 0 ? PlanetStatus.Colonized : PlanetStatus.Inhabitable;

        public bool IsValidCoord(HexTileCoord coord) => _tileMap.IsValidCoord(coord);

        public HexTile GetHexTile(HexTileCoord coord) => _tileMap.GetHexTile(coord);

        public T GetTileObject<T>(HexTileCoord coord) where T : IOnHexTileObject =>
            _tileMap.GetTileObject<T>(coord);

        public IReadOnlyList<T> GetTileObjectList<T>() where T : IOnHexTileObject =>
            _tileMap.GetTileObjectList<T>();

        public IReadOnlyDictionary<Building, int> GetAroundBuildings(HexTileCoord coord)
        {
            var result = new Dictionary<Building, int>();

            foreach (var c in _tileMap.GetRing(1, coord))
            {
                var building = GetTileObject<Building>(c);
                if (building == null) continue;

                if (result.ContainsKey(building))
                    result[building]++;
                else
                    result[building] = 1;
            }

            return result;
        }

        public IEnumerator<HexTile> GetEnumerator() => _tileMap.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
