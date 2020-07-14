using Infinity.HexTileMap;
using UnityEngine;

namespace Infinity.PlanetPop
{
    public class PlanetWrapper : MonoBehaviour
    {
        private Planet planet;

        [SerializeField]
        private TileMapWrapper tileMapPrefab;

        public void Init(Planet p)
        {
            planet = p;
            name = planet.Name;
        }

        private void Start()
        {
            var tileMap = Instantiate(tileMapPrefab, transform);
            tileMap.Init(planet.TileMap, planet.EventHandler);
        }
    }
}