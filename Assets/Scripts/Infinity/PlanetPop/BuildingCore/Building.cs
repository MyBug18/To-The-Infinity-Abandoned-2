using Infinity.HexTileMap;
using System;
using System.Collections.Generic;
using Infinity.GameData;

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

        public Building(Neuron parentNeuron, BuildingPrototype prototype)
        {
            _neuron = parentNeuron.GetChildNeuron(this);
            SignalDispatcher = new SignalDispatcher(_neuron);

            Name = prototype.Name;

            var slotData = GameDataStorage.Instance.GetGameData<PopSlotData>();

            foreach (var kv in prototype.BasePopSlots)
            {
                var slot = new PopWorkingSlot(_neuron, slotData[kv.Key]);
                for (var i = 0; i < kv.Value; i++)
                    _popSlots.Add(slot);
            }
        }
    }
}
