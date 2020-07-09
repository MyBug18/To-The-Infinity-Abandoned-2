namespace Infinity.Modifier
{
    public interface IModifierAttachable
    {
        void AddModifier(BasicModifier modifier);

        void RemoveModifier(BasicModifier modifier);
    }
}
