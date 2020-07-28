using System;

namespace Infinity
{
    public class GameEventSignal<TTarget> : ISignal where TTarget : ISignalDispatcherHolder
    {
        public ISignalDispatcherHolder SignalSender { get; }

        public string Command { get; }
    }
}