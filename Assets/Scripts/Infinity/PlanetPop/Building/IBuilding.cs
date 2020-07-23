using System.Collections.Generic;
using Infinity.HexTileMap;

namespace Infinity.PlanetPop.Building
{
    public interface IBuilding : IOnHexTileObject
    {
        public IReadOnlyList<PopSlot> Slots { get; }
    }
}