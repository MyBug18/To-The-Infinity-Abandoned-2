using System;
using System.Collections;
using System.Collections.Generic;
using Infinity.HexTileMap;

namespace Infinity.GalaxySystem
{
    public class Galaxy : ITileMapHolder
    {
        private readonly TileMap _tileMap;

        public int TileMapRadius => _tileMap.Radius;

        Type ISignalDispatcherHolder.HolderType => typeof(StarSystem);
        public SignalDispatcher SignalDispatcher { get; }

        private readonly Neuron _neuron;

        public TileMapType TileMapType => TileMapType.Galaxy;

        public IReadOnlyList<IOnHexTileObject> this[HexTileCoord coord] => _tileMap[coord];

        public Galaxy(Neuron parentNeuron)
        {
            _neuron = parentNeuron.GetChildNeuron(this);
            SignalDispatcher = new SignalDispatcher(_neuron);

            _tileMap = new TileMap(6, _neuron);
        }

        public bool IsValidCoord(HexTileCoord coord) => _tileMap.IsValidCoord(coord);

        public T GetTileObject<T>(HexTileCoord coord) where T : IOnHexTileObject =>
            _tileMap.GetTileObject<T>(coord);

        public IReadOnlyList<T> GetTileObjectList<T>() where T : IOnHexTileObject =>
            _tileMap.GetTileObjectList<T>();

        public IEnumerator<HexTile> GetEnumerator()
        {
            return _tileMap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}