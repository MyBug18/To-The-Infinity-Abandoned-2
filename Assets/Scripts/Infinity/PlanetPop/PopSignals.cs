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

        public readonly PopWorkingSlot DestinationSlot;

        public PopToTrainingCenterSignal(ISignalDispatcherHolder sender, Pop pop, PopWorkingSlot destinationSlot)
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

        public readonly PopWorkingSlot AssignedSlot;

        public PopSlotAssignedSignal(ISignalDispatcherHolder sender, Pop pop, PopWorkingSlot slot)
        {
            SignalSender = sender;
            Pop = pop;
            AssignedSlot = slot;
        }
    }
}