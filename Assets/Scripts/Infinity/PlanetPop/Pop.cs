using System.Collections.Generic;
using Infinity.Modifiers;

namespace Infinity.PlanetPop
{
    public class Pop : IModifierHolder
    {
        private readonly Dictionary<string, BasicModifier> _modifiers = new Dictionary<string, BasicModifier>();

        public IReadOnlyDictionary<string, BasicModifier> Modifiers => _modifiers;

        public string Name { get; private set; }
    }
}