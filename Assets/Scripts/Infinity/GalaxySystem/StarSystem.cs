namespace Infinity.GalaxySystem
{
    public enum StarType
    {
        G,
        BlackHole
    }

    public class StarSystem
    {
        public readonly StarType StarType;

        public readonly EventHandler StarSystemEventHandler;
    }
}