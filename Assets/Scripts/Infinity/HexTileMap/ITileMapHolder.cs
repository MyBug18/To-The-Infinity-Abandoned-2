using System.Collections.Generic;

namespace Infinity.HexTileMap
{
    public enum TileMapType
    {
        Planet,
        StarSystem,
        Galaxy,
    }

    public interface ITileMapHolder : IEnumerable<HexTile>, IEventHandlerHolder
    {
        int TileMapRadius { get; }

        bool IsValidCoord(HexTileCoord coord);

        TileMapType TileMapType { get; }

        T GetTileObject<T>(HexTileCoord coord) where T : IOnHexTileObject;

        IReadOnlyList<IOnHexTileObject> GetAllTileObjects(HexTileCoord coord);

        IReadOnlyCollection<T> GetTileObjectCollection<T>() where T : IOnHexTileObject;
    }
}