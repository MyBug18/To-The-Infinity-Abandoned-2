using System.Collections.Generic;

namespace Infinity.Modifiers
{
    public interface IModifierHolder
    {
        IReadOnlyDictionary<string, BasicModifier> Modifiers { get; }
    }
}
