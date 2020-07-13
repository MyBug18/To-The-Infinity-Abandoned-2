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

    public class ResourceTank
    {
        public float Energy { get; private set; }

        public float Mineral { get; private set; }

        public float Food { get; private set; }

        public float Alloy { get; private set; }

        private EventHandler eventHandler;

        public ResourceTank(EventHandler ev)
        {
            eventHandler = ev;
        }

        public void ChangeResource(ResourceType type, float value)
        {
            switch (type)
            {
                case ResourceType.Energy:
                    Energy += value;
                    break;
                case ResourceType.Mineral:
                    Mineral += value;
                    break;
                case ResourceType.Food:
                    Food += value;
                    break;
                case ResourceType.Alloy:
                    Alloy += value;
                    break;
                default:
                    throw new NotImplementedException("There are not planetary resource type such as " + type + "!");
            }

            // TODO: publish event (maybe)
        }

        public void ApplyTurnResource(Dictionary<ResourceType, float> turnResource)
        {
            Energy += turnResource[ResourceType.Energy];
            Mineral += turnResource[ResourceType.Mineral];
            Food += turnResource[ResourceType.Food];
            Alloy += turnResource[ResourceType.Alloy];
        }

        public static ResourceTank operator -(ResourceTank a)
        {
            return new ResourceTank(a.eventHandler)
            {
                Energy = -a.Energy,
                Mineral = -a.Mineral,
                Food = -a.Food,
                Alloy = -a.Alloy
            };
        }

        public static ResourceTank operator +(ResourceTank a, ResourceTank b)
        {
            return new ResourceTank(a.eventHandler)
            {
                Energy = a.Energy + b.Energy,
                Mineral = a.Mineral + b.Mineral,
                Food = a.Food + b.Food,
                Alloy = a.Alloy + b.Alloy,
                eventHandler = a.eventHandler
            };
        }

        public static ResourceTank operator -(ResourceTank a, ResourceTank b)
        {
            return a + -b;
        }
    }
}
