namespace Infinity
{
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum GameFactorType
    {
        Energy,
        Mineral,
        Food,
        Alloy,
        Money,
        PopGrowth,
        SocietyResearch,
        EngineerResearch,
        Unity,
    }

    public static class GameFactorExtention
    {
        public static bool IsPlanetaryResource(this GameFactorType factorType)
        {
            return GameFactorType.Energy <= factorType && factorType <= GameFactorType.Alloy;
        }
    }
}
