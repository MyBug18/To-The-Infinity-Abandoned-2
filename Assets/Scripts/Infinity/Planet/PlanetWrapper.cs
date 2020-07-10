using Infinity.HexTileMap;
using UnityEngine;

namespace Infinity.Planet
{
    public class PlanetWrapper : MonoBehaviour
    {
        private Planet planet;

        [SerializeField]
        private TileMapWrapper tileMapPrefab;

        public void Init(Planet planet)
        {
            this.planet = planet;
            name = planet.Name;
        }

        private void Start()
        {
            var tileMap = Instantiate(tileMapPrefab, transform);
            tileMap.Init(planet.TileMap);
        }
    }
}