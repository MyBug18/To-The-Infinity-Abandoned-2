using System;

namespace Infinity.Planet
{
    public enum PlanetaryResourceType
    {
        Energy,
        Mineral,
        Food,
        Alloy,
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

        public void ChangeResource(PlanetaryResourceType type, float value)
        {
            switch (type)
            {
                case PlanetaryResourceType.Energy:
                    Energy += value;
                    break;
                case PlanetaryResourceType.Mineral:
                    Mineral += value;
                    break;
                case PlanetaryResourceType.Food:
                    Food += value;
                    break;
                case PlanetaryResourceType.Alloy:
                    Alloy += value;
                    break;
                default:
                    throw new NotImplementedException("There are not resource type such as " + type + "!");
            }

            // TODO: publish event (maybe)
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
