namespace Infinity.HexTileMap
{
    public class TileClickEvent : Event
    {
        public TileMapType TileType { get; private set; }
        public HexTileCoord Coord { get; private set; }

        public static TileClickEvent Create(HexTileCoord c, TileMapType type)
        {
            return new TileClickEvent {Coord = c, TileType = type};
        }

        public override void Dispose()
        {
        }
    }
}