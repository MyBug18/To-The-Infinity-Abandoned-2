using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Infinity.HexTileMap;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Infinity.PlanetPop.BuildingCore
{
    public class BuildingPrototype
    {
        private enum TokenType
        {
            Parenthesis,
            Not,
            Equal,
            And,
            Or,
            Word
        }

        public readonly string Name;

        public readonly int BaseConstructTime;

        public readonly int BaseConstructCost;

        private readonly List<PopSlotPrototype> _slots = new List<PopSlotPrototype>();
        public IReadOnlyList<PopSlotPrototype> Slots => _slots;

        private readonly Dictionary<string, object> _conditions;
        public IReadOnlyDictionary<string, object> Conditions => _conditions;

        private readonly Func<(Planet, HexTileCoord), bool> _conditionChecker;

        public BuildingPrototype(string jsonData)
        {
            var primary = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);

            Name = (string)primary["Name"];
            BaseConstructTime = Convert.ToInt32(primary["BaseConstructTime"]);
            BaseConstructCost = Convert.ToInt32(primary["BaseConstructCost"]);

            foreach (var slotData in JArray.FromObject(primary["Slots"]))
            {
                _slots.Add(new PopSlotPrototype(slotData.ToString()));
            }

            _conditions = JObject.FromObject(primary["Condition"]).ToObject<Dictionary<string, object>>();
            if (_conditions == null) return;

            ConstructConditionChecker();
        }

        public List<HexTileCoord> GetBuildableTile(Planet planet)
        {
            return null;
        }

        private void ConstructConditionChecker()
        {
            var sb = new StringBuilder();

            foreach (var kv in _conditions)
            {
                switch (kv.Key)
                {
                    case "MinPopNumber":
                        sb.Append($"{kv.Key} <= Item1.Pops.Count && ");
                        break;
                    case "TileState":
                        var tokens = TokenizeConditionString(Convert.ToString(kv.Value));
                        break;
                }
            }
        }

        private List<(string, TokenType)> TokenizeConditionString(string condition)
        {
            var result = new List<(string, TokenType)>();
            var currentWordToken = new List<char>();

            foreach (var c in condition)
            {
                if ('a' <= c && c <= 'z' || 'A' <= c && c <= 'Z')
                {
                    currentWordToken.Add(c);
                    continue;
                }

                if (!IsValidSpecialCharacter(c))
                    throw new InvalidOperationException($"Unexpected character : {c} in {Name}.");

                if (currentWordToken.Count > 0)
                {
                    result.Add((new string(currentWordToken.ToArray()), TokenType.Word));
                    currentWordToken.Clear();
                }

                switch (c)
                {
                    case '(':
                    case ')':
                        result.Add(($"{c}", TokenType.Parenthesis));
                        break;
                    case '!':
                        result.Add(("!", TokenType.Not));
                        break;
                    case '=':
                        result.Add(("=", TokenType.Equal));
                        break;
                    case '&':
                        result.Add(("&", TokenType.And));
                        break;
                    case '|':
                        result.Add(("|", TokenType.Or));
                        break;
                    case ' ':
                        break;
                }
            }

            return result;
        }

        private bool IsValidSpecialCharacter(char c) => " !&|()=".Contains(c);
    }

    public class PopSlotPrototype
    {
        private readonly List<FactorChangePrototype> _yield;
        public IReadOnlyList<FactorChangePrototype> Yield => _yield;

        private readonly List<FactorChangePrototype> _upkeep;
        public IReadOnlyList<FactorChangePrototype> Upkeep => _upkeep;

        public readonly float Wage;

        public PopSlotPrototype(string jsonData)
        {
            var primary = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);

            _yield = JArray.FromObject(primary["Yield"]).ToObject<List<FactorChangePrototype>>();
            _upkeep = JArray.FromObject(primary["Upkeep"]).ToObject<List<FactorChangePrototype>>();
            Wage = Convert.ToSingle(primary["Wage"]);
        }
    }

    public struct FactorChangePrototype
    {
        public GameFactorType FactorType;
        public float Amount;
    }
}