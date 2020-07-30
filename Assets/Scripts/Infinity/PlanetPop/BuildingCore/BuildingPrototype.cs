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

        public readonly List<PopSlotPrototype> Slots;

        public Func<Planet, bool> ConditionChecker { get; private set; }

        public BuildingPrototype(string jsonData)
        {
            var primary = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);

            Name = (string)primary["Name"];
            BaseConstructTime = (int)(long)primary["BaseConstructTime"];
            BaseConstructCost = (int)(long)primary["BaseConstructCost"];

            Slots = JArray.FromObject(primary["Slots"]).ToObject<List<PopSlotPrototype>>();
        }
    }

    public struct PopSlotPrototype
    {
        public List<FactorChangePrototype> Yield;

        public List<FactorChangePrototype> Upkeep;

        public float Wage;
    }

    public struct FactorChangePrototype
    {
        public GameFactorType FactorType;
        public float Amount;
    }
}