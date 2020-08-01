using System;
using System.Collections.Generic;
using System.Linq;
using Infinity.HexTileMap;
using Infinity.PlanetPop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        public BuildingPrototype(string jsonData)
        {
            var primary = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);

            Name = (string)primary["Name"];
            BaseConstructTime = Convert.ToInt32(primary["BaseConstructTime"]);
            BaseConstructCost = Convert.ToInt32(primary["BaseConstructCost"]);

            _basePopSlots = JObject.FromObject(primary["BaseSlots"]).ToObject<Dictionary<string, int>>();

            _conditions = JObject.FromObject(primary["Condition"]).ToObject<Dictionary<string, object>>();

            if (_conditions.TryGetValue("TileState", out var condition))
                _tileStateChecker = ConditionParser<HexTile>.ParseCondition(Convert.ToString(condition), PlanetTileStateChecker);
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

        private bool PlanetTileStateChecker(string state, HexTile tile)
        {

            if (state == tile.TileClimate) return true;
            if (state == tile.SpecialResource) return true;

            return false;
        }

        private bool CheckAroundBuildingState()
        {
            return true;
        }
    }
}