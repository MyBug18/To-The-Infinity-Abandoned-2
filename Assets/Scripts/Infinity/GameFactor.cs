namespace Infinity
{
    public struct GameFactor
    {
        public readonly bool IsPlanetary;

        public readonly bool IsResource;

        public readonly string FactorName;

        public GameFactor(bool isPlanetary, bool isResource, string factorName)
        {
            IsPlanetary = isPlanetary;
            IsResource = isResource;
            FactorName = factorName;
        }
    }
}
