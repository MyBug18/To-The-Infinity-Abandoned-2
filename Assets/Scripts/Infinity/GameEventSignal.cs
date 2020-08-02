using System;

namespace Infinity
{
    public class GameEventSignal<TTarget> : ISignal where TTarget : ISignalDispatcherHolder
    {
        public ISignalDispatcherHolder SignalSender { get; }

        public Func<TTarget, bool> ConditionChecker { get; }

        public string Command { get; }
    }
}
