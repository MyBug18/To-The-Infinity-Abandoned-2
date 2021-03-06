using Infinity.HexTileMap;

namespace Infinity.PlanetPop
{
    public class UnInhabitablePlanet : IPlanet
    {
        public string Name { get; }

        public HexTileCoord HexCoord { get; }

        public OwnerType OwnerType { get; }

        public string PlanetType { get; }

        public UnInhabitablePlanet(string name, HexTileCoord coord)
        {
            Name = name;
            HexCoord = coord;

            PlanetType = "Barren";
        }

        PlanetStatus IPlanet.GetPlanetStatus() => PlanetStatus.UnInhabitable;
    }
}
