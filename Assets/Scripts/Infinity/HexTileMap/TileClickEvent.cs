namespace Infinity.HexTileMap
{
    public class TileClickEvent : ISignal
    {
        public TileMapType TileMapType { get; }
        public HexTileCoord Coord { get; }

        public ISignalDispatcherHolder Holder { get; }

        public TileClickEvent(ISignalDispatcherHolder holder, TileMapType tileMapType, HexTileCoord coord)
        {
            Holder = holder;

            TileMapType = tileMapType;
            Coord = coord;
        }
    }
}