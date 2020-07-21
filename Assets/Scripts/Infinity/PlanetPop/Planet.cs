using System;
using System.Collections;
using System.Collections.Generic;
using Infinity.HexTileMap;
using Infinity.Modifiers;

namespace Infinity.PlanetPop
{
    /// <summary>
    /// Inhabitable planet
    /// </summary>
    public class Planet : IPlanet, IModifierAttachable, IAffectedByNextTurn, ITileMapHolder
    {
        public string Name { get; private set; }

        public HexTileCoord HexCoord { get; private set; }

        public PlanetType PlanetType { get; }

        public readonly int Size;

        private readonly TileMap _tileMap;

        public int TileMapRadius => _tileMap.Radius;

        public TileMapType TileMapType => TileMapType.Planet;

        public IReadOnlyList<IOnHexTileObject> this[HexTileCoord coord] => _tileMap[coord];

        public EventHandler EventHandler { get; }

        Type IEventHandlerHolder.HolderType => typeof(Planet);

        #region Pop

        private readonly List<Pop> _pops = new List<Pop>();

        private readonly List<Pop> _unemployedPops = new List<Pop>();

        public IReadOnlyList<Pop> Pops => _pops;

        public IReadOnlyList<Pop> UnemployedPops => _unemployedPops;

        public const int BasePopGrowth = 5;

        #endregion

        #region Resources

        private readonly Dictionary<GameFactor, float> _currentPlanetaryResource = new Dictionary<GameFactor, float>();

        private readonly Dictionary<GameFactor, Dictionary<ResourceChangeType, float>> _planetaryTurnResource =
            new Dictionary<GameFactor, Dictionary<ResourceChangeType, float>>();

        public IReadOnlyDictionary<GameFactor, float> CurrentPlanetaryResource => _currentPlanetaryResource;

        public IReadOnlyDictionary<GameFactor, Dictionary<ResourceChangeType, float>> PlanetaryTurnResource => _planetaryTurnResource;

        #endregion

        private readonly Dictionary<string, BasicModifier> _modifiers = new Dictionary<string, BasicModifier>();

        public IReadOnlyDictionary<string, BasicModifier> Modifiers => _modifiers;

        public Planet(EventHandler parentHandler, string name, HexTileCoord coord, int size)
        {
            HexCoord = coord;
            Name = name;
            Size = size;

            PlanetType = PlanetType.Inhabitable;
            EventHandler = parentHandler.GetEventHandler(this);

            // for test
            _tileMap = new TileMap(6, EventHandler);
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
            if (_modifiers.ContainsKey(modifier.ModifierKey)) return;

            _modifiers.Add(modifier.ModifierKey, modifier);
        }

        public void RemoveModifier(BasicModifier modifier)
        {
            _modifiers.Remove(modifier.ModifierKey);
        }

        PlanetStatus IPlanet.GetPlanetStatus() => _pops.Count > 0 ? PlanetStatus.Colonized : PlanetStatus.Inhabitable;

        public bool IsValidCoord(HexTileCoord coord) => _tileMap.IsValidCoord(coord);

        public T GetTileObject<T>(HexTileCoord coord) where T : IOnHexTileObject =>
            _tileMap.GetTileObject<T>(coord);

        public IReadOnlyCollection<T> GetTileObjectCollection<T>() where T : IOnHexTileObject =>
            _tileMap.GetTileObjectCollection<T>();

        public IEnumerator<HexTile> GetEnumerator()
        {
            return _tileMap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
