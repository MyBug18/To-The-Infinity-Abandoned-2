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
        public GameFactor Factor { get; private set; }

        public Pop pop { get; private set; }

        public PopSlotState CurrentState { get; private set; }
    }
}