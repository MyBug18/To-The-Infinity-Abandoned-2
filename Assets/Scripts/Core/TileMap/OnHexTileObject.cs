namespace Infinity.Core
{
    public abstract class OnHexTileObject
    {
        public string Name { get; private set; }

        public readonly HexTileCoord HexCoord;

        public OnHexTileObject(HexTileCoord coord, string name)
        {
            Name = name;
            HexCoord = coord;
        }
    }
}
