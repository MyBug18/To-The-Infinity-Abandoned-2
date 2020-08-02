using Infinity.GameData;

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
        public readonly string Name;

        public Pop pop { get; private set; }

        public PopSlotState CurrentState { get; private set; }
    }
}
