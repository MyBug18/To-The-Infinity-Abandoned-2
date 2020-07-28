﻿using System;
using System.Collections;
using System.Collections.Generic;
using Infinity.HexTileMap;
using Infinity.Modifiers;
using Infinity.PlanetPop.BuildingCore;

namespace Infinity.PlanetPop
{
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

        private readonly Neuron _neuron;

        Type ISignalDispatcherHolder.HolderType => typeof(Planet);

        public SignalDispatcher SignalDispatcher { get; }

        #region Pop

        private readonly List<Pop> _pops = new List<Pop>();

        private readonly List<Pop> _unemployedPops = new List<Pop>();

        private readonly List<Pop> _trainingCenter = new List<Pop>();

        public IReadOnlyList<Pop> Pops => _pops;

        public IReadOnlyList<Pop> UnemployedPops => _unemployedPops;

        public IReadOnlyList<Pop> TrainingCenter => _trainingCenter;

        public const int InitialPopGrowth = 5;

        #endregion

        public IReadOnlyList<Building> Buildings => GetTileObjectList<Building>();

        private readonly Dictionary<string, BasicModifier> _modifiers = new Dictionary<string, BasicModifier>();

        public IReadOnlyDictionary<string, BasicModifier> Modifiers => _modifiers;

        public Planet(Neuron parentNeuron, string name, HexTileCoord coord, int size)
        {
            _neuron = parentNeuron.GetChildNeuron();
            SignalDispatcher = new SignalDispatcher(_neuron);

            _neuron.Subscribe<GameEventSignal<Planet>>(OnGameEventSignal);
            _neuron.Subscribe<NextTurnSignal>(OnNextTurnSignal);
            _neuron.Subscribe<GameFactorChangeSignal>(OnFactorChangeSignal);

            HexCoord = coord;
            Name = name;
            Size = size;
            PlanetType = PlanetType.Inhabitable;

            _tileMap = new TileMap(6, _neuron);
        }

        private void OnNextTurnSignal(ISignal s)
        {
            if (!(s is NextTurnSignal)) return;

            ApplyTurnResource();
        }

        /// <summary>
        /// Planetary resources only
        /// </summary>
        private void ApplyTurnResource()
        {
        }

        private void OnFactorChangeSignal(ISignal s)
        {
            if (!(s is GameFactorChangeSignal fcs)) return;
        }

        private void OnGameEventSignal(ISignal s)
        {
            if (!(s is GameEventSignal<Planet> ges)) return;
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
