using System.Collections.Generic;
using Infinity.HexTileMap;
using Infinity.Modifiers;

namespace Infinity.Planet
{
    public class Planet : OnHexTileObject, IModifierAttachable, IAffectedByNextTurn
    {
        public readonly bool IsInhabitable;

        public readonly int Size;

        public readonly TileMap TileMap;

        public readonly List<Pop> Pops = new List<Pop>();

        public readonly List<Pop> UnemployedPops = new List<Pop>();

        private readonly List<BasicModifier> modifiers = new List<BasicModifier>();

        public Planet(HexTileCoord coord, string name, int size, bool isInhabitable = false) : base(coord, name)
        {
            IsInhabitable = isInhabitable;
            Size = size;

            // for test
            TileMap = new TileMap(4);
        }

        public void AddModifier(BasicModifier modifier)
        {
            modifiers.Add(modifier);
        }

        public void RemoveModifier(BasicModifier modifier)
        {
            modifiers.Remove(modifier);
        }

        public void OnNextTurn()
        {

        }
    }
}
