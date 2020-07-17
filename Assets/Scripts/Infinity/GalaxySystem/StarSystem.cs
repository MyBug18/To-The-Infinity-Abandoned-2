using System;
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

    public class StarSystem : IEventHandlerHolder
    {
        public string Name { get; private set; }

        public readonly StarType StarType;

        public readonly int Size;

        public EventHandler EventHandler { get; }

        public StarSystem(EventHandler parentHandler)
        {
            EventHandler = parentHandler.GetEventHandler(this);
            Size = 6;
        }

        // private void SetPlanets()
        // {
        //     for (var i = 1; i <= Size; i++)
        //     {
        //         IPlanet planet;
        //
        //         var pos = TileMap.GetRandomCoordFromRing(i);
        //
        //         if (i == 3)
        //         {
        //             planet = new Planet("test", pos, 8, EventHandler);
        //         }
        //         else
        //         {
        //             planet = new UnInhabitablePlanet("test_uninhabitable", pos);
        //         }
        //     }
        // }

        Type IEventHandlerHolder.GetHolderType() => typeof(StarSystem);
    }
}