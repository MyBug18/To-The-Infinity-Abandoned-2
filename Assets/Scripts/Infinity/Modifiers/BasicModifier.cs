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

    public abstract class BasicModifier : IAffectedByNextTurn
    {
        public readonly string ModifierKey;

        public int Level { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public int LeftTurn { get; private set; }

        public readonly ModifierHolder TopHolder;

        public readonly AffectedByModifierType ModifierType;

        public void OnNextTurn()
        {
            throw new System.NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            return obj is BasicModifier b && Name.Equals(b.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
