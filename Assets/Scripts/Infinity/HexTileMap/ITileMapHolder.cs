using System.Collections.Generic;

namespace Infinity.HexTileMap
{
    public enum TileMapType
    {
        Planet,
        StarSystem,
        Galaxy,
    }

    public interface ITileMapHolder : IEnumerable<HexTile>, IEventSenderHolder
    {
        int TileMapRadius { get; }

        bool IsValidCoord(HexTileCoord coord);

        TileMapType TileMapType { get; }

        IReadOnlyList<IOnHexTileObject> this[HexTileCoord coord] { get; }

        T GetTileObject<T>(HexTileCoord coord) where T : IOnHexTileObject;

        IReadOnlyList<T> GetTileObjectList<T>() where T : IOnHexTileObject;
    }
}