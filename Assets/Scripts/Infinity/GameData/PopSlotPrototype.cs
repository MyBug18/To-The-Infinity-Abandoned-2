using Infinity.PlanetPop.BuildingCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Infinity.GameData
{
    public class PopSlotPrototype
    {
        public readonly string Name;

        private readonly List<FactorChangePrototype> _yield;
        public IReadOnlyList<FactorChangePrototype> Yield => _yield;

        private readonly List<FactorChangePrototype> _upkeep;
        public IReadOnlyList<FactorChangePrototype> Upkeep => _upkeep;

        public readonly float Wage;

        public PopSlotPrototype(string jsonData)
        {
            var primary = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);

            Name = Convert.ToString(primary["Name"]);
            _yield = JArray.FromObject(primary["Yield"]).ToObject<List<FactorChangePrototype>>();
            _upkeep = JArray.FromObject(primary["Upkeep"]).ToObject<List<FactorChangePrototype>>();
            Wage = Convert.ToSingle(primary["Wage"]);
        }

        public PopSlot GetPopSlot()
        {
            // TODO
            return null;
        }
    }

    public struct FactorChangePrototype
    {
        public GameFactorType FactorType;
        public float Amount;
    }
}
