using System.Collections.Generic;

namespace Infinity.Modifiers
{
    public interface IModifierHolder
    {
        IReadOnlyList<Modifier> Modifiers { get; }
    }
}
