namespace Infinity
{
    public enum GameFactor
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

    public enum PlanetaryResources
    {
        Energy,
        Mineral,
        Food,
        Alloy,
    }

    /// <summary>
    /// When divided by 2, the rest is 0 when the value is Income, 1 when the value is Upkeep.
    /// </summary>
    public enum ResourceChangeType
    {
        JobIncome,
        JobUpkeep,
        TradeSend,
        TradeReceive,
        BuildingIncome,
        BuildingUpkeep,
        ModifierIncome,
        ModifierUpkeep,
    }
}
