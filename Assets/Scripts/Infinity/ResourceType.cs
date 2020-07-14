namespace Infinity
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
