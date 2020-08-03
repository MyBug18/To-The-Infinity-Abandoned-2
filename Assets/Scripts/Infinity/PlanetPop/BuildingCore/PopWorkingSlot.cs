using Infinity.GameData;

namespace Infinity.PlanetPop.BuildingCore
{
    public enum PopSlotState
    {
        Empty,
        Occupied,
        Training,
    }

    public class PopWorkingSlot
    {
        public PopSlotState CurrentState { get; private set; } = PopSlotState.Empty;

        public readonly string Name;

        public Pop pop { get; private set; }
    }
}
