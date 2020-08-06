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

        #endregion Pop

        private List<(Pop pop, PopWorkingSlot slot, int RemainTurn)> _trainingCenter =
            new List<(Pop pop, PopWorkingSlot slot, int RemainTurn)>();

        public IReadOnlyList<(Pop pop, PopWorkingSlot slot, int RemainTurn)> TrainingCenter => _trainingCenter;

        public IReadOnlyList<Building> Buildings => GetTileObjectList<Building>();

        public readonly PlanetBuildingFactory BuildingFactory;

        private readonly Dictionary<string, BasicModifier> _modifiers = new Dictionary<string, BasicModifier>();

        public IReadOnlyDictionary<string, BasicModifier> Modifiers => _modifiers;

        public Planet(Neuron parentNeuron, string name, HexTileCoord coord, int size)
        {
            _neuron = parentNeuron.GetChildNeuron(this);
            SignalDispatcher = new SignalDispatcher(_neuron);

            BuildingFactory = new PlanetBuildingFactory(_neuron, this);

            _neuron.Subscribe<PopStateChangeSignal>(OnPopStateChangeSignal);
            _neuron.Subscribe<BuildingQueueEndedSignal>(OnBuildingQueueEndedSignal);
            _neuron.Subscribe<GameCommandSignal>(OnGameCommandSignal);

            HexCoord = coord;
            Name = name;
            Size = size;
            PlanetType = "Inhabitable";

            _tileMap = new TileMap(6, _neuron);
        }

        /// <summary>
        /// Just for test
        /// </summary>
        public Planet()
        {
            _tileMap = new TileMap(6, null);
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
            _neuron.SendSignal(new PopStateChangeSignal(this, newPop, PopStateChangeType.Birth), SignalDirection.Upward);
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

                _neuron.SendSignal(new PopStateChangeSignal(this, pop, PopStateChangeType.ToJobSlot, slot),
                    SignalDirection.Downward);
            }

            for (var i = removeIdx.Count - 1; i >= 0; i--)
                _trainingCenter.RemoveAt(i);
        }

        private void OnBuildingQueueEndedSignal(ISignal s)
        {
            if (!(s is BuildingQueueEndedSignal bqes)) return;

            var coord = bqes.QueueElement.Coord;

            var building = new Building(_neuron, bqes.QueueElement.Prototype, this, coord);

            _tileMap.AddTileObject(coord, building);

            _neuron.SendSignal(new BuildingConstructedSignal(this, building.Name, coord), SignalDirection.Downward);
        }

        private void OnPopStateChangeSignal(ISignal s)
        {
            if (!(s is PopStateChangeSignal pscs)) return;

            switch (pscs.State)
            {
                case PopStateChangeType.ToTrainingCenter:
                    var trainingTime = GameDataStorage.Instance.GetGameData<PopSlotData>()
                        .GetTrainingTime(pscs.Pop.Aptitude, pscs.DestinationSlot.Name);
                    _trainingCenter.Add((pscs.Pop, pscs.DestinationSlot, trainingTime));
                    return;
            }
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
                var building = GetTileObject<Building>(coord);
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
