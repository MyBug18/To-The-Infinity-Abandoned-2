using System;
using System.Collections.Generic;
using Infinity.Core.Modifier;
using UnityEngine;

namespace Infinity.Core.Planet
{
    public class Planet : OnHexTileObject, IModifierAttachable
    {
        public readonly bool IsInhabitable;

        public readonly List<Pop> pops = new List<Pop>();

        private readonly List<BasicModifier> modifiers = new List<BasicModifier>();

        public Planet(bool isInhabitable)
        {
            IsInhabitable = isInhabitable;
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
