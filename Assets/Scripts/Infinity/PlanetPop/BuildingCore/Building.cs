using Infinity.HexTileMap;
using System;
using System.Collections.Generic;

namespace Infinity.PlanetPop.BuildingCore
{
    public class Building : IOnHexTileObject, ISignalDispatcherHolder
    {
        public HexTileCoord HexCoord { get; }

        public Type HolderType => typeof(Building);

        private readonly Neuron _neuron;

        public SignalDispatcher SignalDispatcher { get; }

        public string Name { get; }

        private readonly List<PopWorkingSlot> _popSlots = new List<PopWorkingSlot>();

        public IReadOnlyList<PopWorkingSlot> PopSlots => _popSlots;

        public Building(Neuron parentNeuron)
        {
            _neuron = parentNeuron.GetChildNeuron(this);
            SignalDispatcher = new SignalDispatcher(_neuron);
        }
    }
}
