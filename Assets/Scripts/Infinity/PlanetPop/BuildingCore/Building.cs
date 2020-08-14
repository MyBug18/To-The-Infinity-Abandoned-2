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
                foreach (var y in _popSlots.SelectMany(s => s.Upkeep))
                {
                    if (!result.ContainsKey(y.FactorType))
                        result.Add(y.FactorType, 0);

                    result[y.FactorType] += y.Amount;
                }

                return result;
            }
        }

        private readonly Dictionary<string, float> _jobYieldMultiplierFromModifier = new Dictionary<string, float>();

        public IReadOnlyDictionary<string, float> JobYieldMultiplierFromModifier => _jobYieldMultiplierFromModifier;

        public readonly IReadOnlyDictionary<string, float> YieldFromBuilding;

        public readonly IReadOnlyDictionary<string, float> UpkeepFromBuilding;

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

            Name = prototype.Name;
            HexCoord = coord;

            _planet = planet;

            YieldFromBuilding = prototype.BuildingYield;
            UpkeepFromBuilding = prototype.BuildingUpkeep;

            var resourceData = GameDataStorage.Instance.GetGameData<GameFactorResourceData>();
            var slotData = GameDataStorage.Instance.GetGameData<PopSlotData>();

            // Initialize yield kind
            var yieldKind = new HashSet<string>();

            foreach (var kv in prototype.BasePopSlots)
                foreach (var k in slotData[kv.Key].YieldFactorKind.Where(x => resourceData.AllResourceSet.Contains(x)))
                    yieldKind.Add(k);

            YieldResourceKind = yieldKind;

            // Initialize modifiers
            if (modifiers != null)
                foreach (var m in modifiers)
                {
                    if (!m.ModifierInfo.GameFactorAmount.Keys.Any(x => x == "AnyResource" || yieldKind.Contains(x)))
                        continue;
                    _modifiers.Add(m);
                    ApplyModifierChange(m, true);
                }

            // Initialize slots
            foreach (var kv in prototype.BasePopSlots)
            {
                var p = slotData[kv.Key];

                var slot = new PopSlot(_neuron, this, p);
                for (var i = 0; i < kv.Value; i++)
                    _popSlots.Add(slot);
            }

            // Initialize adjacency bonus
            var adj = prototype.AdjacencyBonus;

            AdjacencyBonusMaxLevel = adj.MaxLevel;
            AdjacencyBonusPerLevel = adj.BonusPerLevel;
            AdjacencyBonusDict = adj.BonusChangeInfo;

            _neuron.Subscribe<BuildingConstructedSignal>(OnBuildingConstructedSignal);
            _neuron.Subscribe<GameCommandSignal>(OnGameCommandSignal);
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

                    AdjacencyBonusLevel = Math.Min(AdjacencyBonusMaxLevel, AdjacencyBonusLevel + thisLevel * kv.Value);
                }
            }

            if (!bcs.Coord.IsAdjacent(HexCoord, false)) return;

            // For the adjacent buildings
            if (!AdjacencyBonusDict.TryGetValue(bcs.BuildingName, out var level)) return;

            AdjacencyBonusLevel += level;
        }

        private void AddRemoveModifier(Modifier m, bool isAdding)
        {
            _neuron.SendSignal(new ModifierSignal(this, m, isAdding), SignalDirection.Local);
        }

        private void OnModifierSignal(ISignal s)
        {
            if (!(s is ModifierSignal ms)) return;

            var m = ms.Modifier;

            if (ms.IsForTile && !m.AffectedTiles.Contains(HexCoord)) return;

            var isRelevant = m.ModifierInfo.GameFactorAmount.Keys.Any(
                x => x == "AnyResource" || YieldResourceKind.Contains(x));

            if (isRelevant && ms.IsAdding)
            {
                var sameGroup = _modifiers.FindIndex(x => x.ModifierInfo.ModifierGroup == m.ModifierInfo.ModifierGroup);

                if (sameGroup != -1)
                {
                    ApplyModifierChange(_modifiers[sameGroup], false);
                    _modifiers[sameGroup] = m;
                }
                else
                {
                    _modifiers.Add(m);
                }

                ApplyModifierChange(m, true);
            }
            else if (isRelevant && !ms.IsAdding)
            {
                _modifiers.Remove(m);
                ApplyModifierChange(m, false);
            }
            // If trying to add irrelevant modifier, have to remove the modifier with the same group anyway
            else if (ms.IsAdding)
            {
                var sameGroup = _modifiers.FindIndex(x => x.ModifierInfo.ModifierGroup == m.ModifierInfo.ModifierGroup);

                if (sameGroup != -1)
                    _modifiers.RemoveAt(sameGroup);
            }
        }

        private void ApplyModifierChange(Modifier m, bool isAdding)
        {
            foreach (var kv in m.ModifierInfo.GameFactorAmount)
            {
                // Ignore game factor and irrelevant resources
                if (kv.Key != "AnyResource" && !YieldResourceKind.Contains(kv.Key)) continue;

                if (isAdding)
                    _jobYieldMultiplierFromModifier[kv.Key] += kv.Value;
                else
                    _jobYieldMultiplierFromModifier[kv.Key] -= kv.Value;
            }
        }

        private void OnGameCommandSignal(ISignal s)
        {
            if (!(s is GameCommandSignal gcs) || gcs.CommandType != GameCommandType.StartNewTurn) return;

            DecreaseModifierLeftTurn();
        }

        private void DecreaseModifierLeftTurn()
        {
            for (var i = 0; i < _modifiers.Count; i++)
                _modifiers[i] = _modifiers[i].ReduceLeftTurn();
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
