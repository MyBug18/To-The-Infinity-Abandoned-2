using System;
using System.Collections.Generic;

namespace Infinity.Planet
{
    public enum ResourceType
    {
        All, // Only for events
        Energy, // Planet-exclusive
        Mineral,
        Food,
        Alloy,
        Money, // Global resources
        PhysicsResearch,
        SocietyResearch,
        EngineerResearch,
    }

    public enum ResourceChangeType
    {
        JobIncome,
        JobUpkeep,
        TradeSend,
        TradeReceive,
        BuildingIncome,
        BuildingUpkeep,
    }

    public class PlanetResourceHolder
    {
        private EventHandler eventHandler;

        public readonly Dictionary<ResourceType, float> CurrentResource = new Dictionary<ResourceType, float>();

        public readonly Dictionary<ResourceType, float> turnResourceBase = new Dictionary<ResourceType, float>();

        public PlanetResourceHolder(EventHandler ev)
        {
            eventHandler = ev;

            for (var r = ResourceType.Energy; r <= ResourceType.Alloy; r++)
                CurrentResource.Add(r, 0);

            for (var r = ResourceType.Energy; r <= ResourceType.EngineerResearch; r++)
                turnResourceBase.Add(r, 0);
        }
    }
}
