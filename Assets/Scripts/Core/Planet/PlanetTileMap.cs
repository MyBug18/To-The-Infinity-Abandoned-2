using UnityEngine;

namespace Infinity.Core.Planet
{
    public class PlanetTileMap : BasicTileMap
    {
        protected override void BuildTileMap()
        {
            base.BuildTileMap();

#if UNITY_EDITOR
            Debug.Log($"Radius {Radius} tile map has made!");
#endif
        }

        public override void Init(int radius)
        {
            base.Init(radius);

#if UNITY_EDITOR
            Debug.Log($"Initialized with radius {radius}!");
#endif
        }

        private void Start()
        {
            BuildTileMap();
        }
    }
}
