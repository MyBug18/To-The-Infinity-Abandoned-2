using System;
using System.Collections.Generic;
using Infinity.HexTileMap;

namespace Infinity.PlanetPop.BuildingCore
{
    public class Building : IOnHexTileObject, ISignalDispatcherHolder
    {
        public string Name { get; }

        public HexTileCoord HexCoord { get; }

        public Type HolderType => typeof(Building);

        private readonly Neuron _neuron;

        public SignalDispatcher SignalDispatcher { get; }

        private readonly List<PopSlot> _popSlots = new List<PopSlot>();

        public IReadOnlyList<PopSlot> PopSlots => _popSlots;

        public Building(Neuron parentNeuron)
        {
            _neuron = parentNeuron.GetChildNeuron();
            SignalDispatcher = new SignalDispatcher(_neuron);
        }

    }
}