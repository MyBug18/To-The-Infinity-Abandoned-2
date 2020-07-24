namespace Infinity.HexTileMap
{
    public class TileClickEvent : Event
    {
        public TileMapType TileMapType { get; }
        public HexTileCoord Coord { get; }

        public TileClickEvent(IEventSenderHolder holder, TileMapType tileMapType, HexTileCoord coord) : base(holder)
        {
            TileMapType = tileMapType;
            Coord = coord;
        }
    }
}