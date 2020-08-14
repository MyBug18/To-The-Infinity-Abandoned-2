namespace Infinity.HexTileMap
{
    public interface IOnHexTileObject
    {
        string Name { get; }

        HexTileCoord HexCoord { get; }

        OwnerType OwnerType { get; }
    }

    public enum OwnerType
    {
        NoOne,
        Me,
        Enemy,
    }
}
