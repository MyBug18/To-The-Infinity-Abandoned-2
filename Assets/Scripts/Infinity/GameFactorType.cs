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

    public class GameFactorChangeSignal : ISignal
    {
        public ISignalDispatcherHolder SignalSender { get; }

        public readonly GameFactor Change;

        public readonly AddOrRemove AddOrRemove;

        public GameFactorChangeSignal(ISignalDispatcherHolder holder, GameFactor change, AddOrRemove addOrRemove)
        {
            SignalSender = holder;
            Change = change;
            AddOrRemove = addOrRemove;
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

    public enum AddOrRemove
    {
        Add,
        Remove,
    }
}
