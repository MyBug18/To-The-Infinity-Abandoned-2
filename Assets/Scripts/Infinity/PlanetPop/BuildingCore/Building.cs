using System;
using Infinity.HexTileMap;

namespace Infinity.PlanetPop.BuildingCore
{
    public class Building : IOnHexTileObject, ISignalDispatcherHolder
    {
        public string Name { get; }

        public HexTileCoord HexCoord { get; }

        public Type HolderType { get; }

        public SignalDispatcher SignalDispatcher { get; }

    }
}