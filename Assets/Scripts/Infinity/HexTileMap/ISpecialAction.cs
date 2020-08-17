namespace Infinity.HexTileMap
{
    public interface ISpecialAction
    {
        string SpecialActionName { get; }

        bool IsVisible();

        bool IsAvailable(HexTileCoord coord);

        void DoSpecialAction(HexTileCoord coord);
    }
}