namespace Infinity.HexTileMap
{
    public enum TileMapType
    {
        Planet,
        StarSystem,
        Game,
    }

    public interface ITileMapHolder
    {
        TileMap TileMap { get; }
    }
}
