using Infinity.PlanetPop.BuildingCore;

namespace Infinity.PlanetPop
{
    public class PopBirthSignal : ISignal
    {
        public ISignalDispatcherHolder SignalSender { get; }

        public readonly Pop BornPop;

        public PopBirthSignal(ISignalDispatcherHolder sender, Pop pop)
        {
            SignalSender = sender;
            BornPop = pop;
        }
    }

    public class PopToTrainingCenterSignal : ISignal
    {
        public ISignalDispatcherHolder SignalSender { get; }

        public readonly Pop Pop;

        public readonly PopSlot DestinationSlot;

        public PopToTrainingCenterSignal(ISignalDispatcherHolder sender, Pop pop, PopSlot destinationSlot)
        {
            SignalSender = sender;
            Pop = pop;
            DestinationSlot = destinationSlot;
        }
    }

    public class PopSlotAssignedSignal : ISignal
    {
        public ISignalDispatcherHolder SignalSender { get; }

        public readonly Pop Pop;

        public readonly PopSlot AssignedSlot;

        public PopSlotAssignedSignal(ISignalDispatcherHolder sender, Pop pop, PopSlot slot)
        {
            SignalSender = sender;
            Pop = pop;
            AssignedSlot = slot;
        }
    }
}