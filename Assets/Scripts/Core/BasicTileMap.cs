using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Infinity.Core
{
    public class BasicTileMap : MonoBehaviour
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
                    throw new InvalidOperationException("Given coordinate is invalid: " + coord);

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
        public virtual void BuildTileMap(int radius)
        {
            this.Radius = radius;
            tileMap = new List<HexTile>[2 * radius + 1];

            for (var q = 0; q <= 2 * radius; q++)
            {
                tileMap[q] = new List<HexTile>();
                for (var r = 0; r <= 2 * radius; r++)
                {
                    if (IsValidCoord(q, r))
                    {
                        Debug.Log((q, r));
                        var tile = Instantiate(tilePrefab, transform);
                        tile.Init(q, r);
                        var sqr3 = Mathf.Sqrt(3);
                        tile.transform.localPosition =
                            new Vector3(sqr3 * q + sqr3 * r / 2, 0, 3f * r / 2) + 
                            new Vector3(-radius * 1.5f * sqr3, 0, -radius * 1.5f);

                        tileMap[q].Add(tile);
                    }
                }
            }
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
    }
}
