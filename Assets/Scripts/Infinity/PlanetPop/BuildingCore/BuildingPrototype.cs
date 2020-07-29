using System;
using System.Collections.Generic;

namespace Infinity.PlanetPop.BuildingCore
{
    public class BuildingPrototype
    {
        public string Name { get; private set; }

        public int BaseConstructTime { get; private set; }

        public int BaseConstructCost { get; private set; }

        public Func<Planet, bool> ConditionChecker { get; private set; }

        public readonly List<PopSlotPrototype> Slots = new List<PopSlotPrototype>();
    }

    public struct PopSlotPrototype
    {
        public GameFactor Factor;

        public float Amount;
    }
}