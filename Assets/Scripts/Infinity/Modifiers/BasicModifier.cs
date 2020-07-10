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
        public readonly string Name;

        public readonly string Description;

        public int LeftTurn { get; private set; }

        public readonly ModifierHolder TopHolder;

        public readonly AffectedByModifierType ModifierType;

        public void OnNextTurn()
        {
            throw new System.NotImplementedException();
        }
    }
}
