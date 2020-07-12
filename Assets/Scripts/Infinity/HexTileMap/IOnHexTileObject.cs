using Infinity.Modifiers;

namespace Infinity.HexTileMap
{
    public interface IOnHexTileObject
    {
        string Name { get; }

        HexTileCoord HexCoord { get; }

        void ApplyModifier(BasicModifier modifier);
    }
}
