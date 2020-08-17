using Infinity.GalaxySystem;
using Infinity.PlanetPop;
using UnityEngine;

namespace Tester
{
    public class StarSystemTester : MonoBehaviour
    {
        [SerializeField]
        private StarSystemWrapper prefab;

        private void Start()
        {
            var system = new StarSystem(null);

            foreach (var p in system.TileMap.GetTileObjectList<IPlanet>())
            {
                Debug.Log(p.GetPlanetStatus());
            }
            Instantiate(prefab, transform).Init(system);
        }
    }
}
