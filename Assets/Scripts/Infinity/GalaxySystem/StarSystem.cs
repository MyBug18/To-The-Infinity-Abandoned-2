using Infinity.HexTileMap;
using Infinity.PlanetPop;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Infinity.GalaxySystem
{
    public enum StarType
    {
        G,
        BlackHole
    }

    public class StarSystem : ITileMapHolder, IOnHexTileObject
    {
        public string Name { get; private set; }
        public HexTileCoord HexCoord { get; }

        public readonly StarType StarType;

        public readonly int Size;

        private readonly TileMap _tileMap;

        public int TileMapRadius => _tileMap.Radius;

        public TileMapType TileMapType => TileMapType.StarSystem;

        public IReadOnlyList<IOnHexTileObject> this[HexTileCoord coord] => _tileMap[coord];

        Type ISignalDispatcherHolder.HolderType => typeof(StarSystem);

        public SignalDispatcher SignalDispatcher { get; }

        private readonly Neuron _neuron;

        public StarSystem(Neuron parentNeuron)
        {
            _neuron = parentNeuron.GetChildNeuron(this);
            SignalDispatcher = new SignalDispatcher(_neuron);

            Size = 6;
            Name = "TestSystem";
            HexCoord = new HexTileCoord(3, 3);

            _tileMap = new TileMap(6, _neuron);
            SetPlanets();
        }

        private void SetPlanets()
        {
            for (var i = 1; i <= Size; i++)
            {
                IPlanet planet;

                var pos = _tileMap.GetRandomCoordFromRing(i);

                if (i == 3)
                {
                    planet = new Planet(_neuron, "test", pos, 8);
                }
                else
                {
                    planet = new UnInhabitablePlanet("test_uninhabitable", pos);
                }

                _tileMap.AddTileObject(pos, planet);
            }
        }

        public bool IsValidCoord(HexTileCoord coord) => _tileMap.IsValidCoord(coord);

        public HexTile GetHexTile(HexTileCoord coord) => _tileMap.GetHexTile(coord);

        public T GetTileObject<T>(HexTileCoord coord) where T : IOnHexTileObject =>
            _tileMap.GetTileObject<T>(coord);

        public IReadOnlyList<T> GetTileObjectList<T>() where T : IOnHexTileObject =>
            _tileMap.GetTileObjectList<T>();

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
