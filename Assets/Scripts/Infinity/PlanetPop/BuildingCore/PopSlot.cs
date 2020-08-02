namespace Infinity.PlanetPop.BuildingCore
{
    public enum PopSlotState
    {
        Empty,
        Occupied,
        Training,
    }

    public class PopSlot
    {
        public GameFactorType FactorType { get; private set; }

        public float Amount { get; private set; }

        public Pop pop { get; private set; }

        public PopSlotState CurrentState { get; private set; }
    }
}
