using System;
using System.Collections.Generic;

namespace Infinity.Core.Modifier
{
    public enum ModifierHolder
    {
        None,
        Game,
        Planet,
        IndividualTile,
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
