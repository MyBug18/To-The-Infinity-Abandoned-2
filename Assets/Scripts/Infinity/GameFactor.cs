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

    public static class GameFactorExtention
    {
        public static bool IsPlanetaryResource(this GameFactor factor)
        {
            return GameFactor.Energy <= factor && factor <= GameFactor.Alloy;
        }
    }

    public class FactorChangeSignal : ISignal
    {
        public ISignalDispatcherHolder Holder { get; }

        public readonly FactorChange Change;

        public FactorChangeSignal(ISignalDispatcherHolder holder, FactorChange change)
        {
            Holder = holder;
            Change = change;
        }
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
