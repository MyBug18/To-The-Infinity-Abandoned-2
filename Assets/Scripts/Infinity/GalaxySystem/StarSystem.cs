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
        public string Name { get; private set; }

        public readonly StarType StarType;

        public EventHandler EventHandler { get; }

        private readonly List<IPlanet> _planets = new List<IPlanet>();

        public StarSystem(EventHandler parentHandler)
        {
            EventHandler = parentHandler.GetEventHandler(this);
        }

        Type IEventHandlerHolder.GetHolderType() => typeof(StarSystem);
    }
}