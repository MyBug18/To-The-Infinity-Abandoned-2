namespace Infinity.Modifiers
{
    public interface IModifierAttachable
    {
        void AddModifier(BasicModifier modifier);

        void RemoveModifier(BasicModifier modifier);
    }
}
