using Infinity.Modifiers;

namespace Infinity.HexTileMap
{
    public abstract class OnHexTileObject
    {
        public string Name { get; private set; }

        public HexTileCoord HexCoord { get; private set; }

        protected OnHexTileObject(HexTileCoord coord, string name)
        {
            Name = name;
            HexCoord = coord;
        }

        public abstract void ApplyModifier(BasicModifier modifier);
    }
}
