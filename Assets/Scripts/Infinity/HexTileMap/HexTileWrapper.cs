using System;
using UnityEngine;

namespace Infinity.HexTileMap
{
    public class HexTileWrapper : MonoBehaviour, IClickable
    {
        private HexTileCoord _coord;

        private event Action<HexTileCoord> OnClick;

        public void Init(HexTile t, Action<HexTileCoord> onClicked)
        {
            _coord = t.Coord;
            OnClick += onClicked;
            name = $"Tile ({_coord.Q}, {_coord.R})";
        }

        void IClickable.OnClick()
        {
            OnClick?.Invoke(_coord);
        }
    }
}