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

        Type IEventHandlerHolder.HolderType => typeof(StarSystem);

        public EventHandler EventHandler { get; }

        public TileMapType TileMapType => TileMapType.Galaxy;

        public IReadOnlyList<IOnHexTileObject> this[HexTileCoord coord] => _tileMap[coord];

        public Galaxy(EventHandler parentHandler)
        {
            EventHandler = parentHandler.GetEventHandler(this);
            _tileMap = new TileMap(6, EventHandler);
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