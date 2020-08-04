using System;

namespace Infinity
{
    public class GameFactorChange
    {
        public float Amount => _amountGetter.Invoke();

        public readonly string FactorType;

        private readonly Func<float> _amountGetter;

        public GameFactorChange(Func<float> amountGetter, string factorType)
        {
            _amountGetter = amountGetter;
            FactorType = factorType;
        }
    }
}
