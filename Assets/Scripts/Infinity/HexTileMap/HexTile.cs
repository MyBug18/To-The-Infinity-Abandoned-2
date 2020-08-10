using System;
using System.Collections.Generic;
using System.Linq;
using Infinity.Modifiers;

namespace Infinity.HexTileMap
{
    /// <summary>
    /// Clockwise tile direction
    /// </summary>
    public enum TileDirection
    {
        Right = 0,      // (+1,  0)
        UpRight = 1,    // ( 0, +1)
        UpLeft = 2,     // (-1, +1)
        Left = 3,       // (-1,  0)
        DownLeft = 4,   // ( 0, -1)
        DownRight = 5,  // (+1, -1)
    }

    public readonly struct HexTileCoord : IEquatable<HexTileCoord>
    {
        public readonly int Q;
        public readonly int R;

        public HexTileCoord(int q, int r)
        {
            Q = q;
            R = r;
        }

        public readonly static HexTileCoord Right = new HexTileCoord(1, 0);
        public readonly static HexTileCoord UpRight = new HexTileCoord(0, 1);
        public readonly static HexTileCoord UpLeft = new HexTileCoord(-1, 1);
        public readonly static HexTileCoord Left = new HexTileCoord(-1, 0);
        public readonly static HexTileCoord DownLeft = new HexTileCoord(0, -1);
        public readonly static HexTileCoord DownRight = new HexTileCoord(1, -1);

        public readonly static HashSet<HexTileCoord> AllDirectionSet = new HashSet<HexTileCoord>
        {
            Right,
            UpRight,
            UpLeft,
            Left,
            DownLeft,
            DownRight,
        };

        public override string ToString() => $"({Q}, {R})";

        public bool Equals(HexTileCoord c) => c.Q == Q && c.R == R;

        public override bool Equals(object obj) => (obj is HexTileCoord c) && Equals(c);

        public override int GetHashCode() => base.GetHashCode();

        public static HexTileCoord operator +(HexTileCoord coord) => coord;

        public static HexTileCoord operator -(HexTileCoord coord) => new HexTileCoord(-coord.Q, -coord.R);

        public static HexTileCoord operator +(HexTileCoord coord1, HexTileCoord coord2) =>
            new HexTileCoord(coord1.Q + coord2.Q, coord1.R + coord2.R);

        public static HexTileCoord operator -(HexTileCoord coord1, HexTileCoord coord2) => coord1 + (-coord2);

        public static bool operator ==(HexTileCoord coord1, HexTileCoord coord2) => coord1.Equals(coord2);

        public static bool operator !=(HexTileCoord coord1, HexTileCoord coord2) => !coord1.Equals(coord2);

        public bool IsAdjacent(HexTileCoord coord, bool includeCenter)
        {
            if (this == coord)
                return includeCenter;

            return AllDirectionSet.Contains(this - coord);
        }

        public HexTileCoord AddDirection(TileDirection dir)
        {
            switch (dir)
            {
                case TileDirection.Right:
                    return this + Right;
                case TileDirection.UpRight:
                    return this + UpRight;
                case TileDirection.UpLeft:
                    return this + UpLeft;
                case TileDirection.Left:
                    return this + Left;
                case TileDirection.DownLeft:
                    return this + DownLeft;
                case TileDirection.DownRight:
                    return this + DownRight;
                default:
                    throw new InvalidOperationException();
            }
        }
    }

    public class HexTile : IModifierHolder
    {
        public readonly HexTileCoord Coord;

        public readonly string TileClimate;

        public readonly string SpecialResource;

        public int WayCost { get; private set; } = 1;

        private List<Modifier> _modifiers = new List<Modifier>();

        public IReadOnlyList<Modifier> Modifiers => _modifiers;

        public HexTile(HexTileCoord coord, Neuron tilemapNeuron)
        {
            Coord = coord;

            tilemapNeuron.Subscribe<ModifierSignal>(OnModifierSignal);
        }

        private void OnModifierSignal(ISignal s)
        {
            if (!(s is ModifierSignal ms)) return;

            if (!ms.IsForTile) return;

            if (!ms.Modifier.AffectedTiles.Contains(Coord)) return;

            _modifiers.Add(ms.Modifier);
        }
    }
}
