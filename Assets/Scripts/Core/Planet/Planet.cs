using System;
using System.Collections.Generic;
using Infinity.Core.Modifier;
using UnityEngine;

namespace Infinity.Core.Planet
{
    public class Planet : OnHexTileObject, IModifierAttachable
    {
        public bool IsInhabitable { get; private set; }

        public int Size { get; private set; }

        public readonly List<Pop> pops = new List<Pop>();

        private readonly List<BasicModifier> modifiers = new List<BasicModifier>();

        public PlanetTileMap TileMap { get; private set; }

        [SerializeField]
        private PlanetTileMap tileMapPrefab;

        public void Init(bool isInhabitable, int size)
        {
            IsInhabitable = isInhabitable;
            Size = size;

            TileMap = Instantiate(tileMapPrefab, transform);
            TileMap.Init(Size);
        }

        public void AddModifier(BasicModifier modifier)
        {
            modifiers.Add(modifier);
        }

        public void RemoveModifier(BasicModifier modifier)
        {
            modifiers.Remove(modifier);
        }
    }
}
