using Infinity.HexTileMap;
using System;
using System.Collections;
using System.Collections.Generic;

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

            _neuron.Subscribe<GameEventSignal<Galaxy>>(OnGameEventSignal);

            _tileMap = new TileMap(6, _neuron);
        }

        private void OnGameEventSignal(ISignal s)
        {
            if (!(s is GameEventSignal<Galaxy> ges)) return;
        }

        public bool IsValidCoord(HexTileCoord coord) => _tileMap.IsValidCoord(coord);

        public HexTile GetHexTile(HexTileCoord coord) => _tileMap.GetHexTile(coord);

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
