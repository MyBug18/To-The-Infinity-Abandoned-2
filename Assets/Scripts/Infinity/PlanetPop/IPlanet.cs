using Infinity.HexTileMap;

namespace Infinity.PlanetPop
{

    public enum PlanetStatus
    {
        UnInhabitable,
        Inhabitable,
        Colonized,
    }

    public interface IPlanet : IOnHexTileObject
    {
        string PlanetType { get; }

        PlanetStatus GetPlanetStatus();
    }
}
