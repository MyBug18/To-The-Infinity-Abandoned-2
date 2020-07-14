﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Infinity.HexTileMap
{
    /// <summary>
    /// Clockwise tile direction
    /// </summary>
    public enum TileDirection
    {
        Up,        // (+1,  0)
        UpRight,   // (+1, -1)
        DownRight, // ( 0, -1)
        Down,      // (-1,  0)
        DownLeft,  // (-1, +1)
        UpLeft,    // ( 0, +1)
    }

    public class TileMap : IEnumerable<HexTile>
    {
        private readonly HexTile[][] _tileMap;

        private readonly Dictionary<Type, Dictionary<HexTileCoord, IOnHexTileObject>> _onTileMapObjects =
            new Dictionary<Type, Dictionary<HexTileCoord, IOnHexTileObject>>();

        private EventHandler _planetEventHandler;

        public readonly int Radius;

        public HexTile this[HexTileCoord coord]
        {
            get
            {
                if (!IsValidCoord(coord))
                    return null;

                var q = coord.Q;
                var r = coord.R;

                if (r < Radius)
                    q = q - Radius + r;

                return _tileMap[r][q];
            }
        }

        public TileMap(int radius, EventHandler eh)
        {
            Radius = radius;
            _planetEventHandler = eh;

            _tileMap = new HexTile[radius * 2 + 1][];
            ConstructTileMap();
        }

        private void ConstructTileMap()
        {
            for (var r = 0; r < Radius; r++)
            {
                _tileMap[r] = new HexTile[Radius + r + 1];
                for (var q = Radius - r; q <= 2 * Radius; q++)
                {
                    var qIdx = q - Radius + r;
                    _tileMap[r][qIdx] = new HexTile(new HexTileCoord(q, r));
                }
            }

            for (var r = Radius; r <= 2 * Radius; r++)
            {
                _tileMap[r] = new HexTile[3 * Radius - r + 1];
                for (var q = 0; q <= 3 * Radius - r; q++)
                {
                    _tileMap[r][q] = new HexTile(new HexTileCoord(q, r));
                }
            }
        }

        /// <summary>
        /// Is it valid coordinate on this tile map?
        /// </summary>
        public bool IsValidCoord(HexTileCoord coord)
        {
            return coord.Q + coord.R >= Radius && coord.Q + coord.R <= 3 * Radius;
        }

        public bool IsValidCoord(int q, int r)
        {
            return q + r >= Radius && q + r <= 3 * Radius;
        }

        /// <summary>
        /// Gets OnHexTileObject with given type and HexTileCoord.
        /// </summary>
        /// <returns>Returns null if given type is not in the dict.</returns>
        public T GetTileObjectFromCoord<T>(HexTileCoord coord) where T : IOnHexTileObject
        {
            var type = typeof(T);
            if (!_onTileMapObjects.TryGetValue(type, out var coordObjectDict)) return default;
            if (!coordObjectDict.TryGetValue(coord, out var obj)) return default;

            return (T) obj;
        }

        /// <summary>
        /// Gets collection of OnHexTileObject with given type.
        /// </summary>
        /// <returns>Returns null if given type is not in the dict.</returns>
        public IReadOnlyCollection<T> GetTileObjectCollection<T>() where T : IOnHexTileObject
        {
            var type = typeof(T);
            if (!_onTileMapObjects.TryGetValue(type, out var coordObjectDict)) return null;
            var result = coordObjectDict.Values.Cast<T>();

            return (IReadOnlyCollection<T>)result;
        }

        private void AddTileObject<T>(T onHexTileObject, HexTileCoord coord) where T : IOnHexTileObject
        {
            var type = typeof(T);
            if (!_onTileMapObjects.TryGetValue(type, out var coordObjectDict))
            {
                _onTileMapObjects.Add(type, new Dictionary<HexTileCoord, IOnHexTileObject>());
                coordObjectDict = _onTileMapObjects[type];
            }

            if (coordObjectDict.ContainsKey(coord))
                throw new InvalidOperationException("There are already an OnHexTileObject on the coordinate!");

            coordObjectDict[coord] = onHexTileObject;

            // TODO: publish event (maybe)
        }

        private void RemoveTileObject<T>(HexTileCoord coord)
        {
            var type = typeof(T);
            if (!_onTileMapObjects.TryGetValue(type, out var coordObjectDict)) return;

            if (coordObjectDict.Remove(coord) && coordObjectDict.Count == 0)
                _onTileMapObjects.Remove(type);

            // TODO: publish event (maybe)
        }

        public IEnumerator<HexTile> GetEnumerator()
        {
            return _tileMap.SelectMany(tileArray => tileArray).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}