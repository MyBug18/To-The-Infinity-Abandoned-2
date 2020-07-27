using System;

namespace Infinity
{
    public class FactorChange
    {
        public float Amount => _amountGetter.Invoke();

        public readonly GameFactor Factor;

        private readonly Func<float> _amountGetter;

        public FactorChange(Func<float> amountGetter, GameFactor factor)
        {
            _amountGetter = amountGetter;
            Factor = factor;
        }
    }
}