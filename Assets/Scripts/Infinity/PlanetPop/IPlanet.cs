using Infinity.HexTileMap;

namespace Infinity.PlanetPop
{
    public enum PlanetType
    {
        Gas,
        Barren,
        Frozen,
        Toxic,
        Molten,
        InhabitableBarren,
        Inhabitable,
    }

    public enum PlanetStatus
    {
        UnInhabitable,
        Inhabitable,
        Colonized,
    }

    public interface IPlanet : IOnHexTileObject
    {
        PlanetType PlanetType { get; }

        PlanetStatus GetPlanetStatus();
    }
}