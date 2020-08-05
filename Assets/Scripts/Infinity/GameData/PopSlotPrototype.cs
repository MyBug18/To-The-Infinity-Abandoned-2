using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Infinity.GameData
{
    public class PopSlotPrototype
    {
        public readonly string Name;
        public readonly string Group;

        private readonly List<FactorChangePrototype> _yield;

        public IReadOnlyList<FactorChangePrototype> Yield => _yield;

        private readonly List<FactorChangePrototype> _upkeep;

        public IReadOnlyList<FactorChangePrototype> Upkeep => _upkeep;

        public readonly float Wage;

        public PopSlotPrototype(string jsonData)
        {
            var primary = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);

            Name = Convert.ToString(primary["Name"]);
            Group = Convert.ToString(primary["Group"]);

            _yield = JArray.FromObject(primary["Yield"]).ToObject<List<FactorChangePrototype>>();
            _upkeep = JArray.FromObject(primary["Upkeep"]).ToObject<List<FactorChangePrototype>>();
            Wage = Convert.ToSingle(primary["Wage"]);
        }
    }

    public struct FactorChangePrototype
    {
        public string FactorType;
        public float Amount;
    }
}
