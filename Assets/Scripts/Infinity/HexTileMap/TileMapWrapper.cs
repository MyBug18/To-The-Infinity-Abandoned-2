﻿using System;
using UnityEngine;

namespace Infinity.HexTileMap
{
    public class TileMapWrapper : MonoBehaviour
    {
        [SerializeField]
        private HexTileWrapper hexTilePrefab;

        private TileMap tileMap;

        private EventHandler planetEventHandler;


        public void Init(TileMap t, EventHandler eh)
        {
            tileMap = t;
            planetEventHandler = eh;
            name = "TileMap";
        }

        public void Start()
        {
            ConstructTileMap();
        }

        public void ConstructTileMap()
        {
            if (tileMap == null)
                throw new InvalidOperationException("TileMap has not been initialized!");

            var sqr3 = Mathf.Sqrt(3);
            foreach (var t in tileMap)
            {
                var c = t.Coord;
                var pos = new Vector3(sqr3 * c.Q + sqr3 * c.R / 2, 0, 1.5f * c.R) -
                          new Vector3(tileMap.Radius * 1.5f * sqr3, 0, tileMap.Radius * 1.5f);
                var tile = Instantiate(hexTilePrefab, transform);
                tile.Init(c, OnClickTile);
                tile.transform.localPosition = pos;
            }
        }

        private void OnClickTile(HexTileCoord coord)
        {
            planetEventHandler.Publish(TileClickEvent.Create(coord));
        }
    }
}