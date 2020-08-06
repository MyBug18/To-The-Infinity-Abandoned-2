using Infinity.HexTileMap;
using System;
using System.Collections.Generic;
using Infinity.GameData;

namespace Infinity.PlanetPop.BuildingCore
{
    public class Building : IOnHexTileObject, ISignalDispatcherHolder
    {
        public string Name { get; }

        public HexTileCoord HexCoord { get; }

        public Type HolderType => typeof(Building);

        private readonly Planet _planet;

        private readonly Neuron _neuron;

        public SignalDispatcher SignalDispatcher { get; }

        private readonly List<PopWorkingSlot> _popSlots = new List<PopWorkingSlot>();

        public IReadOnlyList<PopWorkingSlot> PopSlots => _popSlots;

        public IReadOnlyDictionary<string, float> YieldFromJob
        {
            get
            {
                var result = new Dictionary<string, float>();
                foreach (var s in _popSlots)
                {
                    foreach (var y in s.Yield)
                    {
                        if (!result.ContainsKey(y.FactorType))
                            result.Add(y.FactorType, 0);

                        result[y.FactorType] += y.Amount;
                    }
                }

                return result;
            }
        }

        public IReadOnlyDictionary<string, float> UpkeepFromJob
        {
            get
            {
                var result = new Dictionary<string, float>();
                foreach (var s in _popSlots)
                {
                    foreach (var y in s.Upkeep)
                    {
                        if (!result.ContainsKey(y.FactorType))
                            result.Add(y.FactorType, 0);

                        result[y.FactorType] += y.Amount;
                    }
                }

                return result;
            }
        }

        #region AdjacencyBonus

        public int AdjacencyBonusLevel { get; private set; }

        public int AdjacencyBonusMaxLevel { get; private set; }

        public int AdjacencyBonusPerLevel { get; private set; }

        // <Building Name, Change Level>
        public readonly IReadOnlyDictionary<string, int> AdjacencyBonusDict;

        #endregion AdjacencyBonus

        public Building(Neuron parentNeuron, BuildingPrototype prototype, Planet planet, HexTileCoord coord)
        {
            _neuron = parentNeuron.GetChildNeuron(this);
            SignalDispatcher = new SignalDispatcher(_neuron);

            Name = prototype.Name;
            HexCoord = coord;

            _planet = planet;

            var slotData = GameDataStorage.Instance.GetGameData<PopSlotData>();

            foreach (var kv in prototype.BasePopSlots)
            {
                var slot = new PopWorkingSlot(_neuron, slotData[kv.Key]);
                for (var i = 0; i < kv.Value; i++)
                    _popSlots.Add(slot);
            }

            var adj = prototype.AdjacencyBonus;

            AdjacencyBonusMaxLevel = adj.MaxLevel;
            AdjacencyBonusPerLevel = adj.BonusPerLevel;
            AdjacencyBonusDict = adj.BonusChangeInfo;

            _neuron.Subscribe<BuildingConstructedSignal>(OnBuildingConstructedSignal);
        }

        private void OnBuildingConstructedSignal(ISignal s)
        {
            if (!(s is BuildingConstructedSignal bcs)) return;

            // For the constructed building
            if (bcs.Coord == HexCoord)
            {
                foreach (var kv in _planet.GetAroundBuildings(bcs.Coord))
                {
                    if (!AdjacencyBonusDict.TryGetValue(kv.Key.Name, out var thisLevel)) continue;

                    AdjacencyBonusLevel += thisLevel * kv.Value;
                }
            }

            if (!bcs.Coord.IsAdjacent(HexCoord, false)) return;

            // For the adjacent buildings
            if (!AdjacencyBonusDict.TryGetValue(bcs.BuildingName, out var level)) return;

            AdjacencyBonusLevel += level;
        }
    }

    public class BuildingConstructedSignal : ISignal
    {
        public ISignalDispatcherHolder SignalSender { get; }

        public readonly string BuildingName;

        public readonly HexTileCoord Coord;

        public BuildingConstructedSignal(ISignalDispatcherHolder sender, string buildingName, HexTileCoord coord)
        {
            SignalSender = sender;
            BuildingName = buildingName;
            Coord = coord;
        }
    }
}
