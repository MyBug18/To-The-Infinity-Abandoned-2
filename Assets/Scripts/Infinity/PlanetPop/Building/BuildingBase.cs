using System;
using System.Collections.Generic;
using Infinity.HexTileMap;

namespace Infinity.PlanetPop.Building
{
    public abstract class BuildingBase : IOnHexTileObject
    {
        public string Name { get; }

        public HexTileCoord HexCoord { get; }

        public IReadOnlyList<PopSlot> Slots { get; }

        private Action<FactorChange> _changeAdder { get; }
        private Action<FactorChange> _changeRemover { get; }

        protected BuildingBase(Action<FactorChange> changeAdder, Action<FactorChange> changeRemover)
        {
            _changeAdder = changeAdder;
            _changeRemover = changeRemover;
        }

        protected void AddFactorChange(FactorChange change) => _changeAdder(change);

        protected void RemoveFactorChange(FactorChange change) => _changeRemover(change);
    }
}