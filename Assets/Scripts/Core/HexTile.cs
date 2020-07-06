using System;
using Infinity.Core.Modifier;
using System.Collections.Generic;
using UnityEngine;

namespace Infinity.Core
{
    /// <summary>
    /// Clockwise tile direction
    /// </summary>
    public enum TileDirection
    {
        Up,        // (+1,  0)
        UpRight,   // (+1, -1)
        DownRight, // ( 0, -1)
        Down,      // (-1,  0)
        DownLeft,  // (-1, +1)
        UpLeft,    // ( 0, +1)
    }

    public struct HexTileCoord
    {
        public int q;
        public int r;

        public HexTileCoord(int q, int r)
        {
            this.q = q;
            this.r = r;
        }
    }

    public class HexTile : MonoBehaviour, IAffectedByNextTurn, IModifierAttachable, IClickable
    {
        public HexTileCoord HexCoordinate { get; private set; }

        public readonly List<OnHexTileObject> ObjectsOnIt = new List<OnHexTileObject>();

        public bool IsObjectOnIt => ObjectsOnIt.Count > 0;

        private readonly List<BasicModifier> modifiers = new List<BasicModifier>();

        public event Action<HexTile> OnClicked;

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

        public void OnClick()
        {
            Debug.Log($"{HexCoordinate} clicked!");
            OnClicked?.Invoke(this);
        }
    }
}
