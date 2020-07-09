using System.Collections.Generic;
using Infinity.Core.Modifier;

namespace Infinity.Core
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

    public struct HexTileCoord
    {
        public readonly int q;
        public readonly int r;

        public HexTileCoord(int q, int r)
        {
            this.q = q;
            this.r = r;
        }
    }

    public class TileInfo : IModifierAttachable
    {
        public readonly HexTileCoord Coord;

        public readonly List<OnHexTileObject> Objects = new List<OnHexTileObject>();

        private readonly List<BasicModifier> modifiers = new List<BasicModifier>();

        public void AddModifier(BasicModifier modifier)
        {
            modifiers.Add(modifier);
        }

        public void RemoveModifier(BasicModifier modifier)
        {
            modifiers.Remove(modifier);
        }
    }
}