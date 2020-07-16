using System.Collections.Generic;

namespace Infinity.Modifiers
{
    public interface IModifierAttachable
    {
        IReadOnlyDictionary<string, BasicModifier> Modifiers { get; }

        void AddModifier(BasicModifier modifier);

        void RemoveModifier(BasicModifier modifier);
    }
}
