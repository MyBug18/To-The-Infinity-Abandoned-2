using System;
using System.Collections.Generic;
using Infinity.Core.Modifier;

namespace Infinity.Core.Planet
{
    public class Planet : OnHexTileObject, IModifierAttachable, IAffectedByNextTurn
    {
        public bool IsInhabitable { get; private set; }

        public int Size { get; private set; }

        public readonly List<Pop> pops = new List<Pop>();

        public readonly List<Pop> unemployedPops = new List<Pop>();

        private readonly List<BasicModifier> modifiers = new List<BasicModifier>();

        public void Init(bool isInhabitable, int size)
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
