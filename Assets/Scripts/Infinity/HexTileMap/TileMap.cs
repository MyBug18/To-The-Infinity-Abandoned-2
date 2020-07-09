using System.Collections;
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

    public class TileMap : IEnumerable
    {
        private readonly HexTile[][] tileMap;

        public readonly int Radius;

        public HexTile this[HexTileCoord coord]
        {
            get
            {
                if (!IsValidCoord(coord))
                    return null;

                var q = coord.q;
                var r = coord.r;

                if (r < Radius)
                    q = q - Radius + r;

                return tileMap[r][q];
            }
        }

        public TileMap(int radius)
        {
            Radius = radius;
            tileMap = new HexTile[][radius * 2 + 1];
            ConstructTileMap();
        }

        private void ConstructTileMap()
        {
            for (var r = 0; r < Radius; r++)
            {
                tileMap[r] = new HexTile[Radius + r + 1];
                for (var q = Radius - r; q <= 2 * Radius; q++)
                {
                    var qIdx = q - Radius + r;
                    tileMap[r][qIdx] = new HexTile(new HexTileCoord(q, r));
                }
            }

            for (var r = Radius; r <= 2 * Radius; r++)
            {
                tileMap[r] = new HexTile[3 * Radius - r + 1];
                for (var q = 0; q <= 3 * Radius - r; q++)
                {
                    tileMap[r][q] = new HexTile(new HexTileCoord(q, r));
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

        public IEnumerator GetEnumerator()
        {
            return tileMap.SelectMany(tileArray => tileArray).GetEnumerator();
        }
    }
}