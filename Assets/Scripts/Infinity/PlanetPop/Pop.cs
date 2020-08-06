using System;
using Infinity.HexTileMap;
using Infinity.Modifiers;
using System.Collections.Generic;
using Infinity.PlanetPop.BuildingCore;

namespace Infinity.PlanetPop
{
    public class Pop : IModifierHolder, ISignalDispatcherHolder
    {
        public Type HolderType => typeof(Pop);

        private readonly Neuron _neuron;

        public SignalDispatcher SignalDispatcher { get; }

        private readonly Dictionary<string, BasicModifier> _modifiers = new Dictionary<string, BasicModifier>();

        public IReadOnlyDictionary<string, BasicModifier> Modifiers => _modifiers;

        public string Name { get; private set; }

        public readonly string Aptitude;

        public string CurrentJob { get; private set; }

        public HexTileCoord CurrentCoord { get; private set; }

        public const int BaseHappiness = 50;

        public int HappinessAdder { get; private set; }

        public int Happiness => Math.Max(0, Math.Min(100, BaseHappiness + HappinessAdder));

        public int YieldMultiplier => 0;

        public int UpkeepMultiplier => 0;

        public Pop(Neuron parentNeuron, string name, HexTileCoord initialCoord)
        {
            _neuron = parentNeuron.GetChildNeuron(this);
            SignalDispatcher = new SignalDispatcher(_neuron);

            Name = name;
            CurrentCoord = initialCoord;

            _neuron.Subscribe<PopStateChangeSignal>(OnPopStateChangeSignal);
        }

        public void ToTrainingCenter(PopWorkingSlot destinationSlot)
        {
            _neuron.SendSignal(new PopStateChangeSignal(this, this, PopStateChangeType.ToTrainingCenter),
                SignalDirection.Upward);
        }

        private void OnPopStateChangeSignal(ISignal s)
        {
            if (!(s is PopStateChangeSignal pscs)) return;

            switch (pscs.State)
            {
                case PopStateChangeType.ToJobSlot:
                    if (pscs.Pop != this) return;
                    CurrentJob = pscs.DestinationSlot.Name;
                    break;
            }
        }
    }

    public class PopStateChangeSignal : ISignal
    {
        public ISignalDispatcherHolder SignalSender { get; }

        public readonly Pop Pop;

        public readonly PopStateChangeType State;

        public readonly PopWorkingSlot DestinationSlot;

        public PopStateChangeSignal(ISignalDispatcherHolder sender, Pop pop, PopStateChangeType state, PopWorkingSlot slot = null)
        {
            SignalSender = sender;
            Pop = pop;
            State = state;
            DestinationSlot = slot;
        }
    }

    public enum PopStateChangeType
    {
        Birth,
        Killed,
        ToTrainingCenter,
        ToJobSlot,
    }
}
