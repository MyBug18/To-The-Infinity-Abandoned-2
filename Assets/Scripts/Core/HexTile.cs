using System;
using Infinity.Core.Modifier;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

namespace Infinity.Core
{
    public struct HexTileCoord
    {
        public int q;
        public int r;
    }

    public class HexTile : MonoBehaviour, IAffectedByNextTurn, IModifierAttachable
    {
        public HexTileCoord HexCoordinate { get; private set; }

        public readonly List<OnHexTileObject> ObjectsOnIt = new List<OnHexTileObject>();

        public bool IsObjectOnIt => ObjectsOnIt.Count > 0;

        private readonly List<BasicModifier> modifiers = new List<BasicModifier>();

        public void OnNextTurn()
        {

        }

        public void Init(int q, int r)
        {
            HexCoordinate = new HexTileCoord { q = q, r = r };
            gameObject.name = $"HexTile ({q}, {r})";
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
