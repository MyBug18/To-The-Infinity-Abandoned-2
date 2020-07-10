using UnityEngine;

namespace Infinity.HexTileMap
{
    public class HexTileWrapper : MonoBehaviour, IClickable
    {
        private HexTileCoord coord;

        private EventHandler planetEventHandler;

        public void Init(HexTileCoord c, EventHandler eh)
        {
            coord = c;
            planetEventHandler = eh;
            name = $"HexTile ({c.Q}, {c.R})";
        }

        public void OnClick()
        {
            planetEventHandler.Publish(TileClickEvent.Create(coord));
        }
    }
}