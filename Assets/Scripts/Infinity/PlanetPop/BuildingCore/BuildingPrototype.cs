using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Infinity.PlanetPop.BuildingCore
{
    public class BuildingPrototype
    {
        public string Name { get; private set; }

        public int BaseConstructTime { get; private set; }

        public int BaseConstructCost { get; private set; }

        public Func<Planet, bool> ConditionChecker { get; private set; }

        public readonly List<PopSlotPrototype> Slots = new List<PopSlotPrototype>();

        public BuildingPrototype(string jsonData)
        {
            var primary = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);

            Name = (string)primary["name"];
            BaseConstructTime = (int)primary["construct_time"];
            BaseConstructCost = (int)primary["construct_cost"];

            var primarySlotList = (primary["base_slots"] as List<object>).Cast<Dictionary<string, object>>();
            foreach (var o in primarySlotList)
            {
                var yieldList = new List<(GameFactorType FactorType, float Amount)>();
                var upkeepList = new List<(GameFactorType FactorType, float Amount)>();

                var yield = o["yield"] as Dictionary<string, object>;
            }
        }
    }

    public struct PopSlotPrototype
    {
        public List<(GameFactorType FactorType, float Amount)> Yield;

        public List<(GameFactorType FactorType, float Amount)> Upkeep;

        public float Wage;
    }
}