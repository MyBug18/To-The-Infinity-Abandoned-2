﻿namespace Infinity.Modifiers
{
    public enum ModifierHolder
    {
        None,
        Game,
        Planet,
        IndividualTile,
        IndividualPop,
    }

    public enum AffectedByModifierType
    {
        Energy,
        Mineral,
        Food,
        Alloy,
        Money,
        PhysicsResearch,
        SocietyResearch,
        EngineerResearch
    }

    public abstract class BasicModifier
    {
        public readonly string Name;

        public readonly string Description;

        public readonly ModifierHolder TopHolder;

        public readonly AffectedByModifierType ModifierType;
    }
}
