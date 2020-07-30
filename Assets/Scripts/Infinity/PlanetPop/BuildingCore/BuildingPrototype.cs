using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Infinity.PlanetPop.BuildingCore
{
    public class BuildingPrototype
    {
        public readonly string Name;

        public readonly int BaseConstructTime;

        public readonly int BaseConstructCost;

        private readonly List<PopSlotPrototype> _slots = new List<PopSlotPrototype>();
        public IReadOnlyList<PopSlotPrototype> Slots => _slots;

        [JsonIgnore]
        public Func<Planet, bool> ConditionChecker { get; private set; }

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

            ConditionChecker = p => p.Name == "asdf";
        }
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