using System;
using UnityEngine;

namespace Infinity.HexTileMap
{
    public class HexTileWrapper : MonoBehaviour, IClickable
    {
        private HexTileCoord _coord;

        private event Action<HexTileCoord> OnClick;

        public void Init(HexTileCoord c, Action<HexTileCoord> onClicked)
        {
            _coord = c;
            OnClick += onClicked;
            name = $"HexTile ({c.Q}, {c.R})";
        }

        void IClickable.OnClick()
        {
            OnClick?.Invoke(_coord);
        }
    }
}