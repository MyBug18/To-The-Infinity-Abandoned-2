using System.Collections.Generic;
using Infinity.Core.Modifier;
using UnityEngine;

namespace Infinity.Core.Planet
{
    public class Pop : IModifierAttachable
    {
        private readonly List<BasicModifier> modifiers = new List<BasicModifier>();

        public string Name { get; private set; }

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