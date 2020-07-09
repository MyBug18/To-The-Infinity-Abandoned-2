using System.Collections.Generic;
using Infinity.Modifier;

namespace Infinity.HexTileMap
{

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

        public readonly List<OnHexTileObject> OnTileObjects = new List<OnHexTileObject>();

        private readonly List<BasicModifier> modifiers = new List<BasicModifier>();

        public HexTile(HexTileCoord coord)
        {
            Coord = coord;
        }

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