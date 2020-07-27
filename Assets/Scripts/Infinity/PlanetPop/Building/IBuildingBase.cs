using System.Collections.Generic;
using Infinity.HexTileMap;

namespace Infinity.PlanetPop.Building
{
    public interface IBuildingBase : IOnHexTileObject, ISignalDispatcherHolder
    {
        public int BaseConstructTime { get; }

        public int BaseConstructCost { get; }

        public IReadOnlyList<PopSlot> Slots { get; }
    }
}