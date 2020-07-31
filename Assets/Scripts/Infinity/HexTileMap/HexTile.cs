using System;
using System.Collections.Generic;

namespace Infinity.HexTileMap
{

    public readonly struct HexTileCoord
    {
        public readonly int Q;
        public readonly int R;

        public HexTileCoord(int q, int r)
        {
            this.Q = q;
            this.R = r;
        }

        public override string ToString()
        {
            return $"({Q}, {R})";
        }

        public HexTileCoord AddDirection(TileDirection dir)
        {
            switch (dir)
            {
                case TileDirection.Right:
                    return new HexTileCoord(Q + 1, R);
                case TileDirection.UpRight:
                    return new HexTileCoord(Q, R + 1);
                case TileDirection.UpLeft:
                    return new HexTileCoord(Q - 1, R + 1);
                case TileDirection.Left:
                    return new HexTileCoord(Q - 1, R);
                case TileDirection.DownLeft:
                    return new HexTileCoord(Q, R - 1);
                case TileDirection.DownRight:
                    return new HexTileCoord(Q + 1, R - 1);
                default:
                    throw new InvalidOperationException();
            }
        }
    }

    public enum TileBaseType
    {
        Space,
        Ocean,
        Desert,
        Snow,
        Grass,
    }

    public enum TileSpecialType
    {
        Hill,
        Mountain,
        Forest,
        Iron,
        Fertile,
    }

    public class HexTile
    {
        public readonly HexTileCoord Coord;

        public TileBaseType BaseType { get; private set; }

        public int WayCost { get; private set; } = 1;

        public HexTile(HexTileCoord coord)
        {
            Coord = coord;
        }
    }
}