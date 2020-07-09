using System.Collections.Generic;
using Infinity.Core.Modifier;

namespace Infinity.Core
{
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