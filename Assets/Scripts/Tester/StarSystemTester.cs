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

            foreach (var t in system)
            {
                var coord = t.Coord;
            }

            foreach (var p in system.GetTileObjectList<IPlanet>())
            {
                Debug.Log(p.GetPlanetStatus());
            }
            Instantiate(prefab, transform).Init(system);
        }
    }
}