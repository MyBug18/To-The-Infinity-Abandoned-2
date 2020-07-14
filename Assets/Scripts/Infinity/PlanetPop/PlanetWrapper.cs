using Infinity.HexTileMap;
using UnityEngine;

namespace Infinity.PlanetPop
{
    public class PlanetWrapper : MonoBehaviour
    {
        private Planet _planet;

        [SerializeField]
        private TileMapWrapper tileMapPrefab;

        public void Init(Planet p)
        {
            _planet = p;
            name = _planet.Name;
        }

        private void Start()
        {
            var tileMap = Instantiate(tileMapPrefab, transform);
            tileMap.Init(_planet.TileMap, _planet.EventHandler);
        }
    }
}