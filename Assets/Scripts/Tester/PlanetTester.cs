using Infinity.HexTileMap;
using Infinity.PlanetPop;
using UnityEngine;

namespace Tester
{
    public class PlanetTester : MonoBehaviour
    {
        [SerializeField]
        private PlanetWrapper planetPrefab;

        private void Start()
        {
            var planet = new Planet(null, "TestEarth", new HexTileCoord(0, 0), 4);

            var p = Instantiate(planetPrefab, transform);
            p.Init(planet);
        }
    }
}