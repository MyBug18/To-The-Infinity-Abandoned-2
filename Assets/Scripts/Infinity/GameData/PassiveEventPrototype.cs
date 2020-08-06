using System;
using Infinity.PlanetPop;

namespace Infinity.GameData
{
    public class PassiveEventPrototype
    {
        public readonly string Name;

        public readonly string EventOwnerType;

        public readonly int Chance;

        public readonly IPropositionalLogic<Game> GameConditionChecker;

        public readonly IPropositionalLogic<Planet> PlanetConditionChecker;
    }
}