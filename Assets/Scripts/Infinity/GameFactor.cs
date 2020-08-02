using System;

namespace Infinity
{
    public class GameFactor
    {
        public float Amount => _amountGetter.Invoke();

        public readonly GameFactorType FactorType;

        private readonly Func<float> _amountGetter;

        public GameFactor(Func<float> amountGetter, GameFactorType factorType)
        {
            _amountGetter = amountGetter;
            FactorType = factorType;
        }
    }
}
