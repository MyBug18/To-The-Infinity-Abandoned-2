using System.Collections.Generic;

namespace Infinity.Modifiers
{
    public interface IModifierHolder
    {
        IReadOnlyDictionary<string, Modifier> Modifiers { get; }
    }
}
