namespace Infinity.HexTileMap
{
    public class TileClickEvent : Event
    {
        public HexTileCoord Coord { get; private set; }

        public static TileClickEvent Create(HexTileCoord c)
        {
            return new TileClickEvent {Coord = c};
        }

        public override void Dispose()
        {
        }
    }
}