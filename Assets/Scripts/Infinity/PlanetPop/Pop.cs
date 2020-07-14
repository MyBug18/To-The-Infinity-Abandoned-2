using System.Collections.Generic;
using Infinity.Modifiers;

namespace Infinity.PlanetPop
{
    public class Pop : IModifierAttachable
    {
        private readonly List<BasicModifier> _modifiers = new List<BasicModifier>();

        public string Name { get; private set; }

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