using System;
using Infinity.PlanetPop;

namespace Infinity.GameData
{
    public class PassiveEventPrototype
    {
        public readonly string Name;

        public readonly string EventOwnerType;

        public readonly int Chance;

        private readonly IPropositionalLogic<Game> _gameConditionChecker;

        public readonly Func<Game, bool> GameConditionChecker;

        private readonly IPropositionalLogic<Planet> _planetConditionChecker;

        public readonly Func<Planet, bool> PlanetConditionChecker;
    }
}