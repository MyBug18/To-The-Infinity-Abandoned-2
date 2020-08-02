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

        public BuildingPrototype(string jsonData)
        {
            var primary = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);

            Name = (string)primary["Name"];
            BaseConstructTime = Convert.ToInt32(primary["BaseConstructTime"]);
            BaseConstructCost = Convert.ToInt32(primary["BaseConstructCost"]);

            _basePopSlots = JObject.FromObject(primary["BaseSlots"]).ToObject<Dictionary<string, int>>();

            _conditions = JObject.FromObject(primary["Condition"]).ToObject<Dictionary<string, object>>();

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
            if (!_conditions.ContainsKey("TileState")) return true;

            return _tileStateChecker.Evaluate(tile);
        }

        public bool CheckAroundBuildings(Planet planet, HexTileCoord coord)
        {
            if (!_conditions.ContainsKey("AroundBuildings")) return true;

            return _aroundBuildingsChecker.Evaluate((planet, coord));
        }

        private bool PlanetTileStateChecker(string state, HexTile tile)
        {
            if (state == tile.TileClimate) return true;
            if (state == tile.SpecialResource) return true;

            return false;
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
                if (kv.Key.Name == holder.Name)
                {
                    leftValue = kv.Value;
                    break;
                }
            }

            switch (holder.Operator)
            {
                case -1:
                    return leftValue < rightValue;
                case 0:
                    return leftValue == rightValue;
                case 1:
                    return leftValue > rightValue;
            }

            throw new InvalidOperationException();
        }
    }
}
