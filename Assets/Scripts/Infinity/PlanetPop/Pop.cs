using System.Collections.Generic;
using Infinity.Modifiers;

namespace Infinity.PlanetPop
{
    public class Pop : IModifierAttachable
    {
        private readonly Dictionary<string, BasicModifier> _modifiers = new Dictionary<string, BasicModifier>();

        public IReadOnlyDictionary<string, BasicModifier> Modifiers => _modifiers;

        public string Name { get; private set; }

        public void AddModifier(BasicModifier modifier)
        {
            if (_modifiers.ContainsKey(modifier.ModifierKey)) return;

            _modifiers.Add(modifier.ModifierKey, modifier);
        }

        public void RemoveModifier(BasicModifier modifier)
        {
            _modifiers.Remove(modifier.ModifierKey);
        }
    }
}