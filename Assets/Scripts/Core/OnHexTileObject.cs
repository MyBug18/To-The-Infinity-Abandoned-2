using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infinity.Core
{
    public abstract class OnHexTileObject : MonoBehaviour
    {
        public HexTileCoord HexCoord { get; private set; }
    }
}
