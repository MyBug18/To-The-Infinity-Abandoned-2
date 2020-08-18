using Infinity.HexTileMap;
using Infinity.PlanetPop;

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

        public OwnerType OwnerType { get; }

        public readonly StarType StarType;

        public readonly int Size;

        public TileMap TileMap { get; }

        private readonly Neuron _neuron;

        public StarSystem(Neuron parentNeuron)
        {
            _neuron = parentNeuron.LinkNewChildNeuron();

            Size = 6;
            Name = "TestSystem";
            HexCoord = new HexTileCoord(3, 3);

            TileMap = new TileMap(6, _neuron);
            SetPlanets();
        }

        private void SetPlanets()
        {
            for (var i = 1; i <= Size; i++)
            {
                IPlanet planet;

                var pos = TileMap.GetRandomCoordFromRing(i);

                if (i == 3)
                {
                    planet = new Planet(_neuron, "test", pos, 8);
                }
                else
                {
                    planet = new UnInhabitablePlanet("test_uninhabitable", pos);
                }

                _neuron.SendSignal(new TileMapObjectAddSignal(_neuron, typeof(IPlanet), planet, HexCoord),
                    SignalDirection.Local);
            }
        }
    }
}
