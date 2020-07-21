namespace Infinity.HexTileMap
{
    public class TileClickEvent : Event
    {
        public TileMapType TileMapType { get; }
        public HexTileCoord Coord { get; }

        public TileClickEvent(IEventHandlerHolder holder, TileMapType tileMapType, HexTileCoord coord) : base(holder)
        {
            TileMapType = tileMapType;
            Coord = coord;
        }
    }
}