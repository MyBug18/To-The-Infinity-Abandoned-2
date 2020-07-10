using System;
using UnityEngine;

namespace Infinity.HexTileMap
{
    public class HexTileWrapper : MonoBehaviour, IClickable
    {
        private HexTileCoord coord;

        private event Action<HexTileCoord> onClick;

        public void Init(HexTileCoord c, Action<HexTileCoord> onClicked)
        {
            coord = c;
            onClick += onClicked;
            name = $"HexTile ({c.Q}, {c.R})";
        }

        void IClickable.OnClick()
        {
            onClick?.Invoke(coord);
        }
    }
}