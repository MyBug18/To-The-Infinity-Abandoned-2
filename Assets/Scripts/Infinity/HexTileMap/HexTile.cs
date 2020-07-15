using System;
using System.Collections.Generic;
using Infinity.Modifiers;

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

    public enum TileType
    {
        Space,
        Ocean,
        Land
    }

    public class HexTile : IModifierAttachable
    {
        public readonly HexTileCoord Coord;

        public TileType TileType { get; private set; }

        public int WayCost { get; private set; } = 1;

        private readonly List<BasicModifier> _modifiers = new List<BasicModifier>();

        public HexTile(HexTileCoord coord)
        {
            Coord = coord;
        }

        public void AddModifier(BasicModifier modifier)
        {
            _modifiers.Add(modifier);
        }

        public void RemoveModifier(BasicModifier modifier)
        {
            _modifiers.Remove(modifier);
        }
    }
}