namespace Infinity.HexTileMap
{
    public interface IOnHexTileObject
    {
        string Name { get; }

        HexTileCoord HexCoord { get; }
    }
}
