using System;
using Infinity.HexTileMap;
using Infinity.Modifiers;
using System.Collections.Generic;
using Infinity.PlanetPop.BuildingCore;

namespace Infinity.PlanetPop
{
    public class Pop : IModifierHolder
    {
        private readonly List<Modifier> _modifiers = new List<Modifier>();

        private readonly Neuron _planetNeuron;

        private Planet _planet;

        public IReadOnlyList<Modifier> Modifiers => _modifiers;

        public string Name { get; private set; }

        public readonly string Aptitude;

        public string CurrentJob { get; private set; }

        public HexTileCoord CurrentCoord { get; private set; }

        public const int BaseHappiness = 50;

        public int HappinessAdder
        {
            get
            {
                var fromPlanetAmenity = _planet.Amenity / (_planet.Pops.Count / 5f);

                return (int) fromPlanetAmenity;
            }
        }

        public int Happiness => Math.Max(0, Math.Min(100, BaseHappiness + HappinessAdder));

        public int YieldMultiplier => 0;

        public Pop(Planet planet, Neuron planetNeuron, string name, HexTileCoord initialCoord)
        {
            _planet = planet;
            _planetNeuron = planetNeuron;

            Name = name;
            CurrentCoord = initialCoord;

            _planetNeuron.Subscribe<PopSlotAssignedSignal>(OnPopSlotAssignedSignal);
        }

        public void ToTrainingCenter(PopSlot destinationSlot)
        {
            if (destinationSlot.CurrentState != PopSlotState.Empty)
                throw new InvalidOperationException();

            _planetNeuron.SendSignal(new PopToTrainingCenterSignal(_planetNeuron.Holder, this, destinationSlot),
                SignalDirection.Local);
        }

        private void OnPopSlotAssignedSignal(ISignal s)
        {
            if (!(s is PopSlotAssignedSignal psas)) return;
            if (psas.Pop != this) return;
            CurrentJob = psas.AssignedSlot.Name;
        }
    }
}
