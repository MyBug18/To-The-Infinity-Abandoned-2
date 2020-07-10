using Infinity.HexTileMap;
using Infinity.Planet;
using UnityEngine;

namespace Tester
{
    public class PlanetTester : MonoBehaviour
    {
        [SerializeField]
        private PlanetWrapper planetPrefab;

        private void Start()
        {
            var planet = new Planet(new HexTileCoord(0, 0), "TestEarth", 3);

            var p = Instantiate(planetPrefab, transform);
            p.Init(planet);
        }
    }
}