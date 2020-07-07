using UnityEngine;

namespace Infinity.Core
{
    public abstract class OnHexTileObject : MonoBehaviour
    {
        public HexTileCoord HexCoord { get; private set; }

        public virtual void Init(HexTileCoord coord)
        {
            HexCoord = coord;
        }
    }
}
