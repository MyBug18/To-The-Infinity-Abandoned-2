﻿using System;
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

        private readonly List<Modifier> _modifiers = new List<Modifier>();

        public IReadOnlyList<Modifier> Modifiers => _modifiers;

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

            _neuron.Subscribe<PopSlotAssignedSignal>(OnPopSlotAssignedSignal);
        }

        public void ToTrainingCenter(PopSlot destinationSlot)
        {
            if (destinationSlot.CurrentState != PopSlotState.Empty)
                throw new InvalidOperationException();

            _neuron.SendSignal(new PopToTrainingCenterSignal(this, this, destinationSlot),
                SignalDirection.Upward);
        }

        private void OnPopSlotAssignedSignal(ISignal s)
        {
            if (!(s is PopSlotAssignedSignal psas)) return;
            if (psas.Pop != this) return;
            CurrentJob = psas.AssignedSlot.Name;
        }
    }
}
