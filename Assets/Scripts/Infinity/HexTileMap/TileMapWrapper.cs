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
            name = "TileMap";
            _holder = holder;
        }

        public void Start()
        {
            ConstructTileMap();
        }

        public void ConstructTileMap()
        {
            var noiseMap = Noise2d.GenerateNoiseMap(13, 13, 2);
            var walker = 0;

            var sqr3 = Mathf.Sqrt(3);
            foreach (var t in _holder.TileMap)
            {
                var c = t.Coord;
                var pos = new Vector3(sqr3 * c.Q + sqr3 * c.R / 2, 0, 1.5f * c.R) -
                          new Vector3(_holder.TileMap.Radius * 1.5f * sqr3, 0, _holder.TileMap.Radius * 1.5f);
                var tile = Instantiate(hexTilePrefab, transform);
                tile.Init(t, OnClickTile);
                var a = noiseMap[walker / 13, walker % 13];
                tile.GetComponent<SpriteRenderer>().color = new Color(a, a, a);
                tile.transform.localPosition = pos;
                walker++;
            }
        }

        private void OnClickTile(HexTileCoord coord)
        {
        }
    }
}
