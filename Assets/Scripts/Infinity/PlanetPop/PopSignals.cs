using Infinity.PlanetPop.BuildingCore;

namespace Infinity.PlanetPop
{
    public class PopBirthSignal : ISignal
    {
        public Neuron FromNeuron { get; }

        public readonly Pop BornPop;

        public PopBirthSignal(Neuron neuron, Pop pop)
        {
            FromNeuron = neuron;
            BornPop = pop;
        }
    }

    public class PopToTrainingCenterSignal : ISignal
    {
        public Neuron FromNeuron { get; }

        public readonly Pop Pop;

        public readonly PopSlot DestinationSlot;

        public PopToTrainingCenterSignal(Neuron neuron, Pop pop, PopSlot destinationSlot)
        {
            FromNeuron = neuron;
            Pop = pop;
            DestinationSlot = destinationSlot;
        }
    }

    public class PopTrainingStatusChangeSignal : ISignal
    {
        public Neuron FromNeuron { get; }

        public readonly PopSlot DestinationSlot;

        public readonly bool IsQuiting;

        public PopTrainingStatusChangeSignal(Neuron neuron, PopSlot destinationSlot, bool isQuiting)
        {
            FromNeuron = neuron;
            DestinationSlot = destinationSlot;
            IsQuiting = isQuiting;
        }
    }

    public class PopSlotAssignedSignal : ISignal
    {
        public Neuron FromNeuron { get; }

        public readonly Pop Pop;

        public readonly PopSlot AssignedSlot;

        public PopSlotAssignedSignal(Neuron neuron, Pop pop, PopSlot slot)
        {
            FromNeuron = neuron;
            Pop = pop;
            AssignedSlot = slot;
        }
    }
}