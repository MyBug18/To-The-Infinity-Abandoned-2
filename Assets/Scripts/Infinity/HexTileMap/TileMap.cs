using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Infinity.HexTileMap
{
    public class TileMap : IEnumerable<HexTile>
    {
        private readonly HexTile[][] _tileMap;

        private readonly Dictionary<HexTileCoord, Dictionary<Type, IOnHexTileObject>> _onTileMapObjects =
            new Dictionary<HexTileCoord, Dictionary<Type, IOnHexTileObject>>();

        public readonly Neuron HolderNeuron;

        public readonly int Radius;

        public IReadOnlyList<IOnHexTileObject> this[HexTileCoord coord] =>
            IsValidCoord(coord) ? GetAllTileObjects(coord) : null;

        public TileMap(int radius, Neuron holderNeuron)
        {
            Radius = radius;
            HolderNeuron = holderNeuron;

            _tileMap = new HexTile[radius * 2 + 1][];
            ConstructTileMap();

            HolderNeuron.Subscribe<TileMapObjectAddSignal>(OnAddTileObject);
            HolderNeuron.Subscribe<TileMapObjectRemoveSignal>(OnRemoveTileObject);
        }

        private void ConstructTileMap()
        {
            for (var r = 0; r < Radius; r++)
            {
                _tileMap[r] = new HexTile[Radius + r + 1];
                for (var q = Radius - r; q <= 2 * Radius; q++)
                {
                    var qIdx = q - Radius + r;
                    _tileMap[r][qIdx] = new HexTile(new HexTileCoord(q, r), HolderNeuron);
                }
            }

            for (var r = Radius; r <= 2 * Radius; r++)
            {
                _tileMap[r] = new HexTile[3 * Radius - r + 1];
                for (var q = 0; q <= 3 * Radius - r; q++)
                {
                    _tileMap[r][q] = new HexTile(new HexTileCoord(q, r), HolderNeuron);
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

        public HexTile GetHexTile(HexTileCoord coord)
        {
            if (!IsValidCoord(coord))
                return null;

            var q = coord.Q;
            var r = coord.R;

            if (r < Radius)
                q = q - Radius + r;

            return _tileMap[r][q];
        }

        public List<HexTileCoord> GetRing(int radius, HexTileCoord? center = null)
        {
            if (radius < 1)
                throw new InvalidOperationException("Radius of a ring must be bigger than 0!");

            var current = center ?? new HexTileCoord(Radius, Radius);

            var resultList = new List<HexTileCoord>();

            // To the start point
            for (var i = 0; i < radius; i++)
                current = current.AddDirection(TileDirection.Right);

            for (var i = 2; i < 8; i++)
            {
                var walkDir = (TileDirection)(i % 6);
                for (var j = 0; j < radius; j++)
                {
                    // Add only valid coordinates
                    if (IsValidCoord(current))
                        resultList.Add(current);
                    current = current.AddDirection(walkDir);
                }
            }
            return resultList;
        }

        public HexTileCoord GetRandomCoordFromRing(int radius, HexTileCoord? center = null)
        {
            var ring = GetRing(radius, center);
            var count = ring.Count;
            var decider = UnityEngine.Random.value;
            for (var i = 0; i < count; i++)
            {
                var chance = 1.0f / (count - i);
                if (decider < chance)
                    return ring[i];
            }

            throw new InvalidOperationException("Chance generator has broken!");
        }

        /// <summary>
        /// Gets OnHexTileObject with given type and HexTileCoord.
        /// </summary>
        /// <returns>Returns null if given type is not in the dict.</returns>
        public T GetTileObject<T>(HexTileCoord coord) where T : IOnHexTileObject
        {
            var type = typeof(T);
            if (!_onTileMapObjects.TryGetValue(coord, out var coordObjectDict)) return default;
            if (!coordObjectDict.TryGetValue(type, out var obj)) return default;

            return (T)obj;
        }

        public IReadOnlyList<IOnHexTileObject> GetAllTileObjects(HexTileCoord coord)
        {
            return _onTileMapObjects.TryGetValue(coord, out var typeObjDict)
                ? typeObjDict.Values.ToList()
                : new List<IOnHexTileObject>();
        }

        /// <summary>
        /// Gets collection of OnHexTileObject with given type.
        /// </summary>
        /// <returns>Returns null if given type is not in the dict.</returns>
        public IReadOnlyList<T> GetTileObjectList<T>() where T : IOnHexTileObject
        {
            var type = typeof(T);
            var result = new List<T>();
            foreach (var typeObjDict in _onTileMapObjects.Values)
            {
                if (!typeObjDict.TryGetValue(type, out var obj)) continue;
                result.Add((T)obj);
            }

            return result;
        }

        private void OnAddTileObject(ISignal s)
        {
            if (!(s is TileMapObjectAddSignal tmoas)) return;

            if (!_onTileMapObjects.TryGetValue(tmoas.Coord, out var typeObjectDict))
            {
                _onTileMapObjects.Add(tmoas.Coord, new Dictionary<Type, IOnHexTileObject>());
                typeObjectDict = _onTileMapObjects[tmoas.Coord];
            }

            if (typeObjectDict.ContainsKey(tmoas.Type))
                throw new InvalidOperationException($"There are already {nameof(tmoas.Type)} on the coordinate!");

            typeObjectDict[tmoas.Type] = tmoas.TileObject;
        }

        private void OnRemoveTileObject(ISignal s)
        {
            if (!(s is TileMapObjectRemoveSignal tmors)) return;

            if (!_onTileMapObjects.TryGetValue(tmors.Coord, out var typeObjectDict)) return;

            if (typeObjectDict.Remove(tmors.Type) && typeObjectDict.Count == 0)
                _onTileMapObjects.Remove(tmors.Coord);
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

    public class TileMapObjectAddSignal : ISignal
    {
        public Neuron FromNeuron { get; }

        public readonly Type Type;

        public readonly IOnHexTileObject TileObject;

        public readonly HexTileCoord Coord;

        public TileMapObjectAddSignal(Neuron fromNeuron, Type type, IOnHexTileObject tileObject, HexTileCoord coord)
        {
            FromNeuron = fromNeuron;
            Type = type;
            TileObject = tileObject;
            Coord = coord;
        }
    }

    public class TileMapObjectRemoveSignal : ISignal
    {
        public Neuron FromNeuron { get; }

        public readonly Type Type;

        public readonly HexTileCoord Coord;

        public TileMapObjectRemoveSignal(Neuron fromNeuron, Type type, HexTileCoord coord)
        {
            FromNeuron = fromNeuron;
            Type = type;
            Coord = coord;
        }
    }
}
