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

        Type IEventSenderHolder.HolderType => typeof(StarSystem);

        public UIEventSender UIEventSender { get; }

        public TileMapType TileMapType => TileMapType.Galaxy;

        public IReadOnlyList<IOnHexTileObject> this[HexTileCoord coord] => _tileMap[coord];

        public Galaxy(UIEventSender parentSender)
        {
            UIEventSender = parentSender.GetUIEventsender(this);
            _tileMap = new TileMap(6, UIEventSender);
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