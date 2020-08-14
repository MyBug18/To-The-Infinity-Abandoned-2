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

    public static class OwnerTypeExtentionMethod
    {
        public static bool IsEnemy(this OwnerType t1, OwnerType t2)
        {
            if (t1 == OwnerType.NoOne || t2 == OwnerType.NoOne) return false;

            return t1 != t2;
        }
    }
}
