using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinity.HexTileMap;
using Infinity.PlanetPop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Infinity.GameData
{
    public class BuildingPrototype
    {
        private enum TokenType
        {
            ParenthesisOpen,
            ParenthesisClose,
            Not,
            Equal,
            And,
            Or,
            Word
        }

        private static Dictionary<string, TileBaseType> _tileBaseTypeStringDict;

        public readonly string Name;

        public readonly int BaseConstructTime;

        public readonly int BaseConstructCost;

        private readonly Dictionary<string, int> _basePopSlots;
        public IReadOnlyDictionary<string, int> BasePopSlots => _basePopSlots;

        private readonly Dictionary<string, object> _conditions;
        public IReadOnlyDictionary<string, object> Conditions => _conditions;

        private readonly Func<(Planet, HexTileCoord), bool> _conditionChecker;

        public BuildingPrototype(string jsonData)
        {
            if (_tileBaseTypeStringDict == null)
            {
                _tileBaseTypeStringDict = new Dictionary<string, TileBaseType>();
                foreach (var tbt in (TileBaseType[]) Enum.GetValues(typeof(TileBaseType)))
                {
                    _tileBaseTypeStringDict[tbt.ToString()] = tbt;
                }
            }

            var primary = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);

            Name = (string)primary["Name"];
            BaseConstructTime = Convert.ToInt32(primary["BaseConstructTime"]);
            BaseConstructCost = Convert.ToInt32(primary["BaseConstructCost"]);

            _basePopSlots = JObject.FromObject(primary["BaseSlots"]).ToObject<Dictionary<string, int>>();

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
                        result.Add(("(", TokenType.ParenthesisOpen));
                        break;
                    case ')':
                        result.Add((")", TokenType.ParenthesisClose));
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
}