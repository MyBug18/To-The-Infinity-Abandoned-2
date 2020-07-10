﻿using System.Collections.Generic;
using Infinity.Modifiers;

namespace Infinity.HexTileMap
{

    public readonly struct HexTileCoord
    {
        public readonly int Q;
        public readonly int R;

        public HexTileCoord(int q, int r)
        {
            this.Q = q;
            this.R = r;
        }

        public override string ToString()
        {
            return $"({Q}, {R})";
        }
    }

    public enum TileType
    {
        Space,
        Ocean,
        Land
    }

    public class HexTile : IModifierAttachable
    {
        public readonly HexTileCoord Coord;

        public TileType TileType { get; private set; }

        public int WayCost { get; private set; } = 1;

        private readonly List<BasicModifier> modifiers = new List<BasicModifier>();

        public HexTile(HexTileCoord coord)
        {
            Coord = coord;
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