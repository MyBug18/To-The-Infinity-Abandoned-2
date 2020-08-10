using Infinity.HexTileMap;
using System;
using System.Collections.Generic;
using System.Linq;
using Infinity.GameData;
using Infinity.Modifiers;

namespace Infinity.PlanetPop.BuildingCore
{
    public class Building : IOnHexTileObject, ISignalDispatcherHolder, IModifierHolder
    {
        public string Name { get; }

        public HexTileCoord HexCoord { get; }

        public Type HolderType => typeof(Building);

        private readonly Planet _planet;

        private readonly Neuron _neuron;

        private readonly List<Modifier> _modifiers = new List<Modifier>();

        public IReadOnlyList<Modifier> Modifiers => _modifiers;

        public SignalDispatcher SignalDispatcher { get; }

        private readonly List<PopSlot> _popSlots = new List<PopSlot>();

        public IReadOnlyList<PopSlot> PopSlots => _popSlots;

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

        public readonly IReadOnlyCollection<string> YieldResourceKind;

        public Building(Neuron parentNeuron, BuildingPrototype prototype, Planet planet, HexTileCoord coord,
            IReadOnlyList<Modifier> modifiers = null)
        {
            _neuron = parentNeuron.GetChildNeuron(this);
            SignalDispatcher = new SignalDispatcher(_neuron);

            Name = prototype.Name;
            HexCoord = coord;

            _planet = planet;

            var slotData = GameDataStorage.Instance.GetGameData<PopSlotData>();

            var yieldKind = new HashSet<string>();

            foreach (var kv in prototype.BasePopSlots)
            {
                var p = slotData[kv.Key];

                foreach (var k in p.YieldResourceKind)
                    yieldKind.Add(k);

                var slot = new PopSlot(_neuron, p);
                for (var i = 0; i < kv.Value; i++)
                    _popSlots.Add(slot);
            }

            YieldResourceKind = yieldKind;

            var adj = prototype.AdjacencyBonus;

            AdjacencyBonusMaxLevel = adj.MaxLevel;
            AdjacencyBonusPerLevel = adj.BonusPerLevel;
            AdjacencyBonusDict = adj.BonusChangeInfo;

            var resourceData = GameDataStorage.Instance.GetGameData<GameFactorData>();

            if (modifiers != null)
                foreach (var m in modifiers)
                {
                    var isRelevant =
                        m.ModifierInfo.GameFactorMultiplier.Keys.Any(x =>
                            x == "AnyResource" || resourceData.AllResourceList.Contains(x));

                    if (!isRelevant)
                        continue;

                    _modifiers.Add(m);
                    ApplyModifier(m);
                }

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

        private void ApplyModifier(Modifier modifier)
        {

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
