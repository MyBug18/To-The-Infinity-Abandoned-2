using System.Collections.Generic;

namespace Infinity.HexTileMap
{
    public enum TileMapType
    {
        Planet,
        StarSystem,
        Game,
    }

    public interface ITileMapHolder : IEnumerable<HexTile>, ISignalDispatcherHolder
    {
        int TileMapRadius { get; }

        TileMapType TileMapType { get; }

        IReadOnlyList<IOnHexTileObject> this[HexTileCoord coord] { get; }

        bool IsValidCoord(HexTileCoord coord);

        HexTile GetHexTile(HexTileCoord coord);

        T GetTileObject<T>(HexTileCoord coord) where T : IOnHexTileObject;

        IReadOnlyList<T> GetTileObjectList<T>() where T : IOnHexTileObject;
    }
}
