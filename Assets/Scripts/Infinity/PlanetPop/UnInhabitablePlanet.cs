using Infinity.HexTileMap;

namespace Infinity.PlanetPop
{
    public class UnInhabitablePlanet : IPlanet
    {
        public string Name { get; }

        public HexTileCoord HexCoord { get; }

        public PlanetType PlanetType { get; }

        public UnInhabitablePlanet(string name, HexTileCoord coord)
        {
            Name = name;
            HexCoord = coord;

            PlanetType = PlanetType.Barren;
        }

        PlanetStatus IPlanet.GetPlanetStatus() => PlanetStatus.UnInhabitable;
    }
}
