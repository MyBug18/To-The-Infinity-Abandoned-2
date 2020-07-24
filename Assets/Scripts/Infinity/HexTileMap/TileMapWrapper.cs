using System;
using Infinity.PlanetPop;
using UnityEngine;

namespace Infinity.HexTileMap
{
    public class TileMapWrapper : MonoBehaviour
    {
        [SerializeField]
        private HexTileWrapper hexTilePrefab;

        private ITileMapHolder _holder;


        public void Init(ITileMapHolder holder)
        {
            name = holder.TileMapType + " TileMap";
            _holder = holder;
        }

        public void Start()
        {
            ConstructTileMap();
        }

        public void ConstructTileMap()
        {
            var sqr3 = Mathf.Sqrt(3);
            foreach (var t in _holder)
            {
                var c = t.Coord;
                var pos = new Vector3(sqr3 * c.Q + sqr3 * c.R / 2, 0, 1.5f * c.R) -
                          new Vector3(_holder.TileMapRadius * 1.5f * sqr3, 0, _holder.TileMapRadius * 1.5f);
                var tile = Instantiate(hexTilePrefab, transform);
                tile.Init(t, OnClickTile);
                tile.transform.localPosition = pos;
            }
        }

        private void OnClickTile(HexTileCoord coord)
        {
            _holder.UIEventSender.Publish(new TileClickEvent(_holder, _holder.TileMapType, coord));
        }
    }
}