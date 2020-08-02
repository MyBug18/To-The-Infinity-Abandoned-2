using Infinity.HexTileMap;
using UnityEngine;

namespace Infinity.GalaxySystem
{
    public class StarSystemWrapper : MonoBehaviour
    {
        private StarSystem _starSystem;

        [SerializeField]
        private TileMapWrapper tileMapPrefab;

        public void Init(StarSystem starSystem)
        {
            _starSystem = starSystem;
            name = starSystem.Name + " System";
        }

        private void Start()
        {
            var tileMap = Instantiate(tileMapPrefab, transform);
            tileMap.Init(_starSystem);
        }
    }
}
