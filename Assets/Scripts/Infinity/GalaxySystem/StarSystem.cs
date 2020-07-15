using System;
using System.Collections.Generic;
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
        public readonly StarType StarType;

        public readonly EventHandler StarSystemEventHandler;

        private readonly List<IPlanet> _planets = new List<IPlanet>();

        public StarSystem(EventHandler parentHandler)
        {
            StarSystemEventHandler = parentHandler.GetEventHandler(this);
        }

        Type IEventHandlerHolder.GetHolderType() => typeof(StarSystem);
    }
}