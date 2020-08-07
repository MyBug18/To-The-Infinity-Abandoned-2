using Infinity.HexTileMap;
using Infinity.PlanetPop;
using Infinity.PlanetPop.BuildingCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infinity.GameData
{
    public class BuildingPrototype
    {
        public readonly string Name;

        public readonly int BaseConstructTime;

        public readonly int BaseConstructCost;

        private readonly Dictionary<string, int> _basePopSlots;

        public IReadOnlyDictionary<string, int> BasePopSlots => _basePopSlots;

        private readonly Dictionary<string, object> _conditions;

        public IReadOnlyDictionary<string, object> Conditions => _conditions;

        private readonly IPropositionalLogic<HexTile> _tileStateChecker;

        private readonly IPropositionalLogic<(Planet planet, HexTileCoord coord)> _aroundBuildingsChecker;

        public readonly AdjacencyBonusData AdjacencyBonus;

        public BuildingPrototype(string jsonData)
        {
            var primary = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);

            Name = (string)primary["Name"];
            BaseConstructTime = Convert.ToInt32(primary["BaseConstructTime"]);
            BaseConstructCost = Convert.ToInt32(primary["BaseConstructCost"]);

            // TODO: Should check whether the slot really exists
            _basePopSlots = JObject.FromObject(primary["BaseSlots"]).ToObject<Dictionary<string, int>>();

            _conditions = JObject.FromObject(primary["Condition"]).ToObject<Dictionary<string, object>>();

            AdjacencyBonus = new AdjacencyBonusData(Convert.ToString(primary["AdjacencyBonus"]));

            if (_conditions == null)
            {
                _conditions = new Dictionary<string, object>();
                return;
            }

            if (_conditions.TryGetValue("TileState", out var tileStateCondition))
                _tileStateChecker = ConditionParser<HexTile>.ParseCondition(
                    Convert.ToString(tileStateCondition), PlanetTileStateChecker);

            if (_conditions.TryGetValue("AroundBuildings", out var aroundBuildingsCondition))
                _aroundBuildingsChecker =
                    ConditionParser<(Planet planet, HexTileCoord coord)>.ParseCondition(
                        Convert.ToString(aroundBuildingsCondition), PlanetAroundBuildingsChecker);
        }

        public Building GetBuilding()
        {
            // TODO
            return null;
        }

        public List<HexTileCoord> GetBuildableTile(Planet planet)
        {
            return null;
        }

        public bool CheckPopNumber(Planet planet)
        {
            if (!_conditions.TryGetValue("MinPopNumber", out var value)) return true;

            return planet.Pops.Count >= Convert.ToInt32(value);
        }

        public bool CheckTileState(HexTile tile)
        {
            return !_conditions.ContainsKey("TileState") || _tileStateChecker.Evaluate(tile);
        }

        public bool CheckAroundBuildings(Planet planet, HexTileCoord coord)
        {
            return !_conditions.ContainsKey("AroundBuildings") || _aroundBuildingsChecker.Evaluate((planet, coord));
        }

        private bool PlanetTileStateChecker(string state, HexTile tile)
        {
            if (state == tile.TileClimate) return true;
            return state == tile.SpecialResource;
        }

        private bool PlanetAroundBuildingsChecker(string comparisonDataHolder, (Planet planet, HexTileCoord coord) data)
        {
            var holder = ConditionParser.ParseBinaryComparison(comparisonDataHolder);
            var buildingDict = data.planet.GetAroundBuildings(data.coord);

            var leftValue = 0;
            var rightValue = holder.RightValue;

            if (holder.Name == "Any")
                leftValue = buildingDict.Values.Sum();

            foreach (var kv in buildingDict)
            {
                if (kv.Key.Name != holder.Name) continue;
                leftValue = kv.Value;
                break;
            }

            return holder.Operator switch
            {
                -1 => leftValue < rightValue,
                0 => leftValue == rightValue,
                1 => leftValue > rightValue,
                _ => throw new InvalidOperationException()
            };
        }
    }

    public class AdjacencyBonusData
    {
        public readonly int BonusPerLevel;

        public readonly int MaxLevel;

        private readonly Dictionary<string, int> _bonusChangeInfo;

        public IReadOnlyDictionary<string, int> BonusChangeInfo => _bonusChangeInfo;

        public AdjacencyBonusData(string jsonData)
        {
            var primary = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);

            BonusPerLevel = Convert.ToInt32(primary["BonusPerLevel"]);
            MaxLevel = Convert.ToInt32(primary["MaxLevel"]);
            _bonusChangeInfo = JObject.FromObject(primary["BonusChangeInfo"]).ToObject<Dictionary<string, int>>();
        }
    }
}
