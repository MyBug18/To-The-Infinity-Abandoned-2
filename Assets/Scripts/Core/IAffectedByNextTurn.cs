using System;

namespace Infinity.Core
{
    public interface IAffectedByNextTurn
    {
        void OnNextTurn();
    }
}
