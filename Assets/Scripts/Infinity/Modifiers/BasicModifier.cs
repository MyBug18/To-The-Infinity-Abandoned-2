namespace Infinity.Modifiers
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
        public readonly string ModifierKey;

        public int Level { get; private set; }

        public string DescriptionKey { get; private set; }

        public int LeftTurn { get; private set; }

        public readonly ModifierHolder TopHolder;

        public readonly AffectedByModifierType ModifierType;

        public void OnNextTurn()
        {
            throw new System.NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            return obj is BasicModifier b && ModifierKey.Equals(b.ModifierKey);
        }

        public override int GetHashCode()
        {
            return ModifierKey.GetHashCode();
        }
    }
}
