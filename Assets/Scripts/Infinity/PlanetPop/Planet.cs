using Infinity.HexTileMap;
using Infinity.Modifiers;
using Infinity.PlanetPop.BuildingCore;
using System;
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

        public OwnerType OwnerType { get; }

        public string PlanetType { get; }

        public readonly int Size;

        private readonly Neuron _neuron;

        public TileMap TileMap { get; }

        #region Pop

        private readonly List<Pop> _pops = new List<Pop>();

        private readonly List<Pop> _unemployedPops = new List<Pop>();

        public IReadOnlyList<Pop> Pops => _pops;

        public IReadOnlyList<Pop> UnemployedPops => _unemployedPops;

        public const float InitialPopGrowth = 5f;

        public float CurrentPopGrowth { get; private set; }

        private readonly List<(Pop pop, PopSlot slot, int RemainTurn)> _trainingCenter =
            new List<(Pop pop, PopSlot slot, int RemainTurn)>();

        public IReadOnlyList<(Pop pop, PopSlot slot, int RemainTurn)> TrainingCenter => _trainingCenter;

        #endregion Pop

        #region GameFactor

        private readonly Dictionary<string, float> _currentResourceKeep = new Dictionary<string, float>();

        public IReadOnlyDictionary<string, float> CurrentResourceKeep => _currentResourceKeep;

        private Dictionary<string, float> _yieldFromJobCache = new Dictionary<string, float>();

        public IReadOnlyDictionary<string, float> YieldFromJob => _yieldFromJobCache;

        private Dictionary<string, float> _upkeepFromJobCache = new Dictionary<string, float>();

        public IReadOnlyDictionary<string, float> UpkeepFromJob => _upkeepFromJobCache;

        private readonly Dictionary<string, float> _yieldFromBuilding = new Dictionary<string, float>();

        private readonly Dictionary<string, float> _upkeepFromBuilding = new Dictionary<string, float>();

        public float Amenity => _yieldFromJobCache.GetValueOrDefault("Amenity") +
            _yieldFromBuilding.GetValueOrDefault("Amenity") - _pops.Count;

        #endregion GameFactor

        #region Building

        public IReadOnlyList<Building> Buildings => TileMap.GetTileObjectList<Building>();

        public readonly PlanetBuildingFactory BuildingFactory;

        #endregion Building

        private readonly List<Modifier> _modifiers = new List<Modifier>();

        public IReadOnlyList<Modifier> Modifiers => _modifiers;

        public Planet(Neuron parentNeuron, string name, HexTileCoord coord, int size)
        {
            _neuron = parentNeuron.LinkNewChildNeuron();

            BuildingFactory = new PlanetBuildingFactory(_neuron, this);

            _neuron.Subscribe<PopToTrainingCenterSignal>(OnPopToTrainingCenterSignal);
            _neuron.Subscribe<GameCommandSignal>(OnGameCommandSignal);
            _neuron.Subscribe<ModifierSignal>(OnModifierSignal);
            _neuron.Subscribe<BuildingQueueChangeSignal>(OnBuildingQueueChangeSignal);

            _neuron.EventConditionPasser.SetRefiner(OnPassiveEventCheck);

            HexCoord = coord;
            Name = name;
            Size = size;
            PlanetType = "Inhabitable";

            TileMap = new TileMap(6, _neuron);
        }

        /// <summary>
        /// Just for test
        /// </summary>
        public Planet()
        {
            TileMap = new TileMap(6, null);
        }

        private void AddRemoveModifier(Modifier m, bool isAdding)
        {
            _neuron.SendSignal(new ModifierSignal(_neuron, m, isAdding), SignalDirection.Downward);
        }

        private void OnModifierSignal(ISignal s)
        {
            if (!(s is ModifierSignal ms)) return;

            var m = ms.Modifier;

            if (ms.IsForTile) return;

            if (ms.IsAdding)
            {
                var sameGroup = _modifiers.FindIndex(x => x.ModifierInfo.ModifierGroup == m.ModifierInfo.ModifierGroup);

                // If the modifier of same group already exists
                if (sameGroup != -1)
                {
                    // Remove it's effect and replace it with new one
                    ApplyModifierChange(_modifiers[sameGroup], false);
                    _modifiers[sameGroup] = m;
                }
                else
                {
                    _modifiers.Add(m);
                }
            }
            else
            {
                _modifiers.Remove(m);
            }


            ApplyModifierChange(m, ms.IsAdding);
        }

        private void ApplyModifierChange(Modifier m, bool IsAdding)
        {
            var resourceData = GameDataStorage.Instance.GetGameData<GameFactorResourceData>();

            foreach (var kv in m.ModifierInfo.GameFactorAmount)
            {
                if (resourceData.GetFactorKind(kv.Key) != GameFactorKind.PlanetaryFactor) continue;
            }
        }

        private void RecalculateJobResources()
        {
            _yieldFromJobCache = GetBuildingYieldFromJob();
            _upkeepFromJobCache = GetBuildingUpkeepFromJob();
        }

        private Dictionary<string, float> GetBuildingYieldFromJob()
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

        private Dictionary<string, float> GetBuildingUpkeepFromJob()
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

        private void OnGameCommandSignal(ISignal s)
        {
            if (!(s is GameCommandSignal gcs) || gcs.CommandType != GameCommandType.StartNewTurn) return;

            ApplyTurnResource();
            ApplyPopGrowth();
            ProceedTraining();
            DecreaseModifierLeftTurn();

            // Because may trainings have ended
            RecalculateJobResources();
        }

        /// <summary>
        /// Planetary resources only
        /// </summary>
        private void ApplyTurnResource()
        {
            var resourceData = GameDataStorage.Instance.GetGameData<GameFactorResourceData>();

            foreach (var kv in _yieldFromJobCache.Where(kv => resourceData.PlanetaryResourceSet.Contains(kv.Key)))
            {
                if (!_currentResourceKeep.ContainsKey(kv.Key))
                    _currentResourceKeep.Add(kv.Key, 0);

                _currentResourceKeep[kv.Key] += kv.Value;
            }

            foreach (var kv in _yieldFromBuilding.Where(kv => resourceData.PlanetaryResourceSet.Contains(kv.Key)))
            {
                if (!_currentResourceKeep.ContainsKey(kv.Key))
                    _currentResourceKeep.Add(kv.Key, 0);

                _currentResourceKeep[kv.Key] += kv.Value;
            }

            foreach (var kv in _upkeepFromJobCache.Where(kv => resourceData.PlanetaryResourceSet.Contains(kv.Key)))
            {
                if (!_currentResourceKeep.ContainsKey(kv.Key))
                    _currentResourceKeep.Add(kv.Key, 0);

                _currentResourceKeep[kv.Key] -= kv.Value;
            }

            foreach (var kv in _upkeepFromBuilding.Where(kv => resourceData.PlanetaryResourceSet.Contains(kv.Key)))
            {
                if (!_currentResourceKeep.ContainsKey(kv.Key))
                    _currentResourceKeep.Add(kv.Key, 0);

                _currentResourceKeep[kv.Key] -= kv.Value;
            }

            var deficitResources = new List<string>();

            foreach (var kv in _currentResourceKeep)
            {
                if (kv.Value >= 0) continue;

                _currentResourceKeep[kv.Key] = 0;
                deficitResources.Add(kv.Key);
            }

            // TODO: Send resource deficit signal to upward
        }

        private void ApplyPopGrowth()
        {
            var popGrowthMultipliers =
                _yieldFromJobCache.GetValueOrDefault("PopGrowth") + _yieldFromBuilding.GetValueOrDefault("PopGrowth");

            CurrentPopGrowth += InitialPopGrowth * (popGrowthMultipliers / 100f + 1);

            if (CurrentPopGrowth < 100) return;

            var newPop = new Pop(this, _neuron, "TestPop");
            _neuron.SendSignal(new PopBirthSignal(_neuron, newPop), SignalDirection.Upward);
            _unemployedPops.Add(newPop);
        }

        private void ProceedTraining()
        {
            var trainFinishedIdx = new List<int>();

            for (var i = 0; i < _trainingCenter.Count; i++)
            {
                var (pop, slot, remainTurn) = _trainingCenter[i];
                _trainingCenter[i] = (pop, slot, remainTurn - 1);

                if (_trainingCenter[i].RemainTurn != 0) continue;

                trainFinishedIdx.Add(i);

                _neuron.SendSignal(new PopSlotAssignedSignal(_neuron, pop, slot), SignalDirection.Downward);
            }

            if (trainFinishedIdx.Count == 0) return;

            // Because a pop has assigned to a pop slot
            RecalculateJobResources();

            for (var i = trainFinishedIdx.Count - 1; i >= 0; i--)
                _trainingCenter.RemoveAt(i);
        }

        private void DecreaseModifierLeftTurn()
        {
            for (var i = 0; i < _modifiers.Count; i++)
                _modifiers[i] = _modifiers[i].ReduceLeftTurn();
        }

        private void OnPopToTrainingCenterSignal(ISignal s)
        {
            if (!(s is PopToTrainingCenterSignal pttcs)) return;

            var trainingTime = GameDataStorage.Instance.GetGameData<PopSlotData>()
                .GetTrainingTime(pttcs.Pop.Aptitude, pttcs.DestinationSlot.Name);

            _trainingCenter.Add((pttcs.Pop, pttcs.DestinationSlot, trainingTime));
            _neuron.SendSignal(new PopTrainingStatusChangeSignal(_neuron, pttcs.DestinationSlot, false),
                SignalDirection.Downward);

            // Because a pop from pop slot has been emptied
            RecalculateJobResources();
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

        private void OnBuildingQueueChangeSignal(ISignal s)
        {
            if (!(s is BuildingQueueChangeSignal bqcs)) return;

            var element = bqcs.QueueElement;

            switch (bqcs.Type)
            {
                case BuildingQueueChangeType.Added:
                    foreach (var kv in element.Prototype.BaseConstructCost)
                        _currentResourceKeep[kv.Key] -= kv.Value;
                    break;
                case BuildingQueueChangeType.Canceled:
                    foreach (var kv in element.Prototype.BaseConstructCost)
                        _currentResourceKeep[kv.Key] += kv.Value;
                    break;
                case BuildingQueueChangeType.Ended:
                    var coord = element.Coord;

                    var modifiers = _modifiers.ToList();
                    modifiers.AddRange(TileMap.GetHexTile(coord).Modifiers);

                    var building = new Building(_neuron, element.Prototype, this, coord, modifiers);

                    _neuron.SendSignal(new TileMapObjectAddSignal(_neuron, typeof(Building), building, HexCoord),
                        SignalDirection.Local);

                    foreach (var kv in building.YieldFromBuilding)
                    {
                        if (!_yieldFromBuilding.ContainsKey(kv.Key))
                            _yieldFromBuilding.Add(kv.Key, 0);

                        _yieldFromBuilding[kv.Key] += kv.Value;
                    }

                    foreach (var kv in building.UpkeepFromBuilding)
                    {
                        if (!_upkeepFromBuilding.ContainsKey(kv.Key))
                            _upkeepFromBuilding.Add(kv.Key, 0);

                        _upkeepFromBuilding[kv.Key] += kv.Value;
                    }

                    // Because adjacency bonus may have been changed
                    RecalculateJobResources();

                    _neuron.SendSignal(new BuildingConstructedSignal(_neuron, building.Name, coord), SignalDirection.Downward);

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        PlanetStatus IPlanet.GetPlanetStatus() => _pops.Count > 0 ? PlanetStatus.Colonized : PlanetStatus.Inhabitable;

        public IReadOnlyDictionary<Building, int> GetAroundBuildings(HexTileCoord coord)
        {
            var result = new Dictionary<Building, int>();

            foreach (var building in TileMap.GetRing(1, coord)
                .Select(c => TileMap.GetTileObject<Building>(c))
                .Where(building => building != null))
            {
                if (result.ContainsKey(building))
                    result[building]++;
                else
                    result[building] = 1;
            }

            return result;
        }
    }
}
