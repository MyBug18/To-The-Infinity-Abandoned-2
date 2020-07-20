using System;
using System.Collections;
using System.Collections.Generic;
using Infinity.HexTileMap;
using Infinity.PlanetPop;

namespace Infinity.GalaxySystem
{
    public enum StarType
    {
        G,
        BlackHole
    }

    public class StarSystem : ITileMapHolder
    {
        public string Name { get; private set; }

        public readonly StarType StarType;

        public readonly int Size;

        private readonly TileMap _tileMap;

        public int TileMapRadius => _tileMap.Radius;

        public TileMapType TileMapType => TileMapType.StarSystem;

        Type IEventHandlerHolder.HolderType => typeof(StarSystem);

        public EventHandler EventHandler { get; }

        public StarSystem(EventHandler parentHandler)
        {
            EventHandler = new EventHandler(this);
//            EventHandler = parentHandler.GetEventHandler(this);
            Size = 6;
            Name = "TestSystem";

            _tileMap = new TileMap(6, EventHandler);
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
                    planet = new Planet(EventHandler, "test", pos, 8);
                }
                else
                {
                    planet = new UnInhabitablePlanet("test_uninhabitable", pos);
                }

                _tileMap.AddTileObject(pos, planet);
            }
        }

        public bool IsValidCoord(HexTileCoord coord) => _tileMap.IsValidCoord(coord);

        public T GetTileObject<T>(HexTileCoord coord) where T : IOnHexTileObject =>
            _tileMap.GetTileObject<T>(coord);

        public IReadOnlyList<IOnHexTileObject> GetAllTileObjects(HexTileCoord coord) =>
            _tileMap.GetAllTileObjects(coord);

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