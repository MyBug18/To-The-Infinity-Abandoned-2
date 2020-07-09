using UnityEngine;

namespace Infinity.HexTileMap
{
    public class HexTileWrapper : MonoBehaviour, IClickable
    {
        private HexTileCoord coord;

        public void Init(HexTileCoord c)
        {
            coord = c;
        }

        public void OnClick()
        {
            Debug.Log($"({coord.q}, {coord.r}) clicked!");
        }
    }
}