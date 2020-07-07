using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infinity.Core
{
    public abstract class BasicTileMap : MonoBehaviour
    {
        [SerializeField]
        private HexTile tilePrefab;

        private List<HexTile>[] tileMap;

        public int Radius { get; private set; }

        public HexTile this[HexTileCoord coord]
        {
            get
            {
                if (!IsValidCoord(coord))
                    return null;

                var q = coord.q;
                var r = coord.r;
                if (q < Radius)
                    r = coord.r - Radius + q;

                return tileMap[q][r];
            }
        }

        /// <summary>
        /// Builds a tile map with given radius
        /// </summary>
        protected virtual void BuildTileMap()
        {
            tileMap = new List<HexTile>[2 * Radius + 1];

            for (var q = 0; q <= 2 * Radius; q++)
            {
                tileMap[q] = new List<HexTile>();
                for (var r = 0; r <= 2 * Radius; r++)
                {
                    if (!IsValidCoord(q, r)) continue;

                    var tile = Instantiate(tilePrefab, transform);
                    tile.Init(q, r);
                    tile.transform.localPosition = GetActualPosition(new HexTileCoord {q = q, r = r});
                    tile.OnClicked += OnTileClick;

                    tileMap[q].Add(tile);
                }
            }
        }

        public virtual void Init(int radius)
        {
            Radius = radius;
        }

        public Vector3 GetActualPosition(HexTileCoord coord)
        {
            var sqr3 = Mathf.Sqrt(3);
            return new Vector3(sqr3 * coord.q + sqr3 * coord.r / 2, 0, 3f * coord.r / 2) +
                   new Vector3(-Radius * 1.5f * sqr3, 0, -Radius * 1.5f);
        }

        /// <summary>
        /// Is it valid coordinate on this tile map?
        /// </summary>
        public bool IsValidCoord(HexTileCoord coord)
        {
            return coord.q + coord.r >= Radius && coord.q + coord.r <= 3 * Radius;
        }

        public bool IsValidCoord(int q, int r)
        {
            return q + r >= Radius && q + r <= 3 * Radius;
        }

        protected virtual void OnTileClick(HexTile tile)
        {
        }

        public List<HexTileCoord> NeighborCoords(HexTileCoord coord)
        {
            return new List<HexTileCoord>
            {
                new HexTileCoord(coord.q + 1, coord.r),
                new HexTileCoord(coord.q + 1, coord.r - 1),
                new HexTileCoord(coord.q, coord.r - 1),
                new HexTileCoord(coord.q - 1, coord.r),
                new HexTileCoord(coord.q - 1, coord.r + 1),
                new HexTileCoord(coord.q, coord.r + 1)
            };
        }
    }
}
