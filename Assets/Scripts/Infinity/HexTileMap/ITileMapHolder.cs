using System.Collections.Generic;

namespace Infinity.HexTileMap
{
    public interface ITileMapHolder : IEnumerable<HexTile>, IEventHandlerHolder
    {
        int TileMapRadius { get; }

        bool IsValidCoord(HexTileCoord coord);

        T GetTileObject<T>(HexTileCoord coord) where T : IOnHexTileObject;

        IReadOnlyList<IOnHexTileObject> GetAllTileObjects(HexTileCoord coord);

        IReadOnlyCollection<T> GetTileObjectCollection<T>() where T : IOnHexTileObject;
    }
}