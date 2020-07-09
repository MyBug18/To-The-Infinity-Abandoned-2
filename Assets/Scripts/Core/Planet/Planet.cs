using System;
using System.Collections.Generic;
using Infinity.Core.Modifier;

namespace Infinity.Core.Planet
{
    public class Planet : OnHexTileObject, IModifierAttachable, IAffectedByNextTurn
    {
        public readonly bool IsInhabitable;

        public readonly int Size;

        public readonly List<Pop> Pops = new List<Pop>();

        public readonly List<Pop> UnemployedPops = new List<Pop>();

        private readonly List<BasicModifier> modifiers = new List<BasicModifier>();

        public Planet(HexTileCoord coord, string name, int size, bool isInhabitable = false) : base(coord, name)
        {
            IsInhabitable = isInhabitable;
            Size = size;
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
