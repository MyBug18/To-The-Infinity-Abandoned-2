using System;
using UnityEngine;

namespace Infinity.HexTileMap
{
    public class TileMapWrapper : MonoBehaviour
    {
        [SerializeField]
        private HexTileWrapper hexTilePrefab;

        private TileMap _tileMap;

        private EventHandler _planetEventHandler;


        public void Init(TileMap t, EventHandler eh)
        {
            _tileMap = t;
            _planetEventHandler = eh;
            name = "TileMap";
        }

        public void Start()
        {
            ConstructTileMap();
        }

        public void ConstructTileMap()
        {
            if (_tileMap == null)
                throw new InvalidOperationException("TileMap has not been initialized!");

            var sqr3 = Mathf.Sqrt(3);
            foreach (var t in _tileMap)
            {
                var c = t.Coord;
                var pos = new Vector3(sqr3 * c.Q + sqr3 * c.R / 2, 0, 1.5f * c.R) -
                          new Vector3(_tileMap.Radius * 1.5f * sqr3, 0, _tileMap.Radius * 1.5f);
                var tile = Instantiate(hexTilePrefab, transform);
                tile.Init(c, OnClickTile);
                tile.transform.localPosition = pos;
            }
        }

        private void OnClickTile(HexTileCoord coord)
        {
            _planetEventHandler.Publish(TileClickEvent.Create(coord));
        }
    }
}