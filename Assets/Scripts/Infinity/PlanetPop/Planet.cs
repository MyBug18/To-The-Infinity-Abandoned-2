using System;
using System.Collections.Generic;
using Infinity.HexTileMap;
using Infinity.Modifiers;

namespace Infinity.PlanetPop
{
    /// <summary>
    /// Inhabitable planet
    /// </summary>
    public class Planet : IPlanet, IModifierAttachable, IAffectedByNextTurn, IEventHandlerHolder
    {
        public string Name { get; private set; }

        public HexTileCoord HexCoord { get; private set; }

        public PlanetType PlanetType { get; }

        public readonly int Size;

        public readonly TileMap TileMap;

        public readonly EventHandler EventHandler;

        #region Pop

        private readonly List<Pop> _pops = new List<Pop>();

        private readonly List<Pop> _unemployedPops = new List<Pop>();

        public IReadOnlyList<Pop> Pops => _pops;

        public IReadOnlyList<Pop> UnemployedPops => _unemployedPops;

        public const int BasePopGrowth = 3;

        #endregion

        #region Resources

        private readonly Dictionary<ResourceType, float> _currentPlanetaryResource = new Dictionary<ResourceType, float>();

        private readonly Dictionary<ResourceType, Dictionary<ResourceChangeType, float>> _planetaryTurnResource =
            new Dictionary<ResourceType, Dictionary<ResourceChangeType, float>>();

        public IReadOnlyDictionary<ResourceType, float> CurrentPlanetaryResource => _currentPlanetaryResource;

        public IReadOnlyDictionary<ResourceType, Dictionary<ResourceChangeType, float>> PlanetaryTurnResource => _planetaryTurnResource;

        #endregion

        private readonly List<BasicModifier> _modifiers = new List<BasicModifier>();

        public Planet(string name, HexTileCoord coord, int size)
        {
            HexCoord = coord;
            Name = name;
            Size = size;

            PlanetType = PlanetType.Inhabitable;

            // for test
            EventHandler = new EventHandler(this);
            EventHandler.Subscribe<TileClickEvent>(OnTileClickEvent);

            // for test
            TileMap = new TileMap(4, EventHandler);
        }

        public void OnNextTurn()
        {
            ApplyTurnResource();
        }

        /// <summary>
        /// Planetary resources only
        /// </summary>
        private void ApplyTurnResource()
        {
        }

        public void ApplyModifier(BasicModifier modifier)
        {

        }

        public void AddModifier(BasicModifier modifier)
        {
            _modifiers.Add(modifier);
        }

        public void RemoveModifier(BasicModifier modifier)
        {
            _modifiers.Remove(modifier);
        }

        private void OnTileClickEvent(Event e, IEventHandlerHolder holder)
        {
            if (!(e is TileClickEvent tce)) return;

            var coord = tce.Coord;
            UnityEngine.Debug.Log("Clicked ( " + coord + " )!");
        }

        Type IEventHandlerHolder.GetHolderType()
        {
            return typeof(Planet);
        }

        PlanetStatus IPlanet.GetPlanetStatus() => _pops.Count > 0 ? PlanetStatus.Colonized : PlanetStatus.Inhabitable;
    }
}
