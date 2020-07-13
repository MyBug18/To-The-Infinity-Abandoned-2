using System;
using System.Collections.Generic;

namespace Infinity
{
    public abstract class Event : IDisposable
    {
        public abstract void Dispose();
    }

    public readonly struct EventInfo
    {
        public readonly Action<Event> Callback;

        public readonly bool ReceiveOnlyOnce;

        public EventInfo(Action<Event> c, bool once)
        {
            Callback = c;
            ReceiveOnlyOnce = once;
        }

        public override bool Equals(object obj)
        {
            return obj is Action<Event> c && c == Callback;
        }

        public override int GetHashCode()
        {
            return Callback.GetHashCode();
        }
    }

    /// <summary>
    /// Super simple event handler
    /// </summary>
    public class EventHandler
    {
        private readonly Dictionary<Type, List<EventInfo>> subscribeInfoDict = new Dictionary<Type, List<EventInfo>>();

        public void Subscribe<T>(Action<Event> c, bool once = false) where T : Event
        {
            var type = typeof(T);
            var eventInfo = new EventInfo(c, once);

            // Make new list and add it when there are no initial list for the given type
            if (!subscribeInfoDict.ContainsKey(type))
            {
                var list = new List<EventInfo>();
                subscribeInfoDict.Add(type, list);
            }

            subscribeInfoDict[type].Add(eventInfo);
        }

        public void UnSubscribe<T>(Action<Event> c) where T : Event
        {
            var type = typeof(T);

            if (subscribeInfoDict.TryGetValue(type, out var infos))
            {
                var idx = infos.FindIndex(i => i.Callback.Equals(c));
                if (idx == -1) return;

                infos.RemoveAt(idx);
                if (infos.Count == 0)
                    subscribeInfoDict.Remove(type);
            }
        }

        public void Publish<T>(T e) where T : Event
        {
            var type = typeof(T);
            var removeList = new List<EventInfo>();

            if (!subscribeInfoDict.TryGetValue(type, out var infos)) return;

            foreach (var i in infos)
            {
                i.Callback.Invoke(e);

                if (i.ReceiveOnlyOnce)
                    removeList.Add(i);
            }

            foreach (var i in removeList)
                infos.Remove(i);

            if (infos.Count == 0)
                subscribeInfoDict.Remove(type);

            e.Dispose();
        }
    }
}