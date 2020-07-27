namespace Infinity.PlanetPop.Building
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

        public Pop pop { get; private set; }

        public PopSlotState CurrentState { get; private set; }
    }
}