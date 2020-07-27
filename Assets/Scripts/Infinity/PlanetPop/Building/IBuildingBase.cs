using System.Collections.Generic;
using Infinity.HexTileMap;

namespace Infinity.PlanetPop.Building
{
    public interface IBuildingBase : IOnHexTileObject, ISignalDispatcherHolder
    {
        public IReadOnlyList<PopSlot> Slots { get; }
    }
}