﻿using System;
using System.Collections;
using System.Collections.Generic;
using Infinity.HexTileMap;
using Infinity.Modifiers;
using Infinity.PlanetPop.Building;

namespace Infinity.PlanetPop
{
    public enum PlanetaryResourceType
    {
        Energy,
        Mineral,
        Food,
        Alloy,
    }

    /// <summary>
    /// Inhabitable planet
    /// </summary>
    public class Planet : IPlanet, IModifierHolder, ITileMapHolder
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

        private readonly List<Pop> _trainingCenter = new List<Pop>();

        public IReadOnlyList<Pop> Pops => _pops;

        public IReadOnlyList<Pop> UnemployedPops => _unemployedPops;

        public IReadOnlyList<Pop> TrainingCenter => _trainingCenter;

        public const int InitialPopGrowth = 5;

        #endregion

        public IReadOnlyList<BuildingBase> Buildings => GetTileObjectList<BuildingBase>();

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

        private void AddFactorChange(FactorChange change)
        {

        }

        private void RemoveFactorChange(FactorChange change)
        {

        }

        private void ToTrainingCenter(Pop pop)
        {
            _trainingCenter.Add(pop);
        }

        PlanetStatus IPlanet.GetPlanetStatus() => _pops.Count > 0 ? PlanetStatus.Colonized : PlanetStatus.Inhabitable;

        public bool IsValidCoord(HexTileCoord coord) => _tileMap.IsValidCoord(coord);

        public T GetTileObject<T>(HexTileCoord coord) where T : IOnHexTileObject =>
            _tileMap.GetTileObject<T>(coord);

        public IReadOnlyList<T> GetTileObjectList<T>() where T : IOnHexTileObject =>
            _tileMap.GetTileObjectList<T>();

        public IEnumerator<HexTile> GetEnumerator() =>  _tileMap.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}