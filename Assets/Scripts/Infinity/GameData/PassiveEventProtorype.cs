using System;

namespace Infinity.GameData
{
    public class PassiveEventProtorype
    {
        public readonly string Name;

        public readonly string EventOwnerType;

        public readonly IPropositionalLogic<Game> GameConditionChecker;
    }
}