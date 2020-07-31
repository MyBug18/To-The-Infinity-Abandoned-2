using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Infinity.GameData
{
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