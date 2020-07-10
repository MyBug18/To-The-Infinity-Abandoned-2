using UnityEngine;

namespace Infinity.HexTileMap
{
    public class HexTileWrapper : MonoBehaviour, IClickable
    {
        private HexTileCoord coord;

        public void Init(HexTileCoord c)
        {
            coord = c;
            name = $"HexTile ({c.Q}, {c.R})";
        }

        public void OnClick()
        {
            Debug.Log($"({coord.Q}, {coord.R}) clicked!");
        }
    }
}