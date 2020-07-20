using System;
using Infinity.GalaxySystem;
using Infinity.HexTileMap;
using Infinity.PlanetPop;
using UnityEngine;

namespace Infinity
{
    public class WrapperMaker : MonoBehaviour
    {
        public static WrapperMaker Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        [SerializeField]
        private StarSystemWrapper _starSystemWrapper;

        [SerializeField]
        private TileMapWrapper _tileMapWrapper;

        [SerializeField]
        private PlanetWrapper _planetWrapper;

        [SerializeField]
        private HexTileWrapper _hexTileWrapper;
    }
}