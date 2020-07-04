using System;
using UnityEngine;

namespace Infinity.Core
{
    public struct HexTileCoord
    {
        public int q;
        public int r;
    }

    public class HexTile : MonoBehaviour, IAffectedByNextTurn
    {
        public HexTileCoord HexCoordinate { get; private set; }

        public OnHexTileObject ObjectOnIt { get; set; }

        public void OnNextTurn()
        {

        }

        public void Init(int q, int r)
        {
            HexCoordinate = new HexTileCoord { q = q, r = r };
            gameObject.name = $"HexTile ({q}, {r})";
        }
    }
}
