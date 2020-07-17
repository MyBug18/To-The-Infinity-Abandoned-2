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
        public readonly Action<Event, IEventHandlerHolder> Callback;

        public readonly bool ReceiveOnlyOnce;

        public EventInfo(Action<Event, IEventHandlerHolder> c, bool once)
        {
            Callback = c;
            ReceiveOnlyOnce = once;
        }

        public override bool Equals(object obj)
        {
            return obj is Action<Event, IEventHandlerHolder> c && c == Callback;
        }

        public override int GetHashCode()
        {
            return Callback.GetHashCode();
        }
    }

    public interface IEventHandlerHolder
    {
        Type HolderType { get; }
        EventHandler EventHandler { get; }
    }

    /// <summary>
    /// Super simple event handler with hierarchy
    /// </summary>
    public class EventHandler
    {
        private readonly Dictionary<Type, List<EventInfo>> _subscribeInfoDict = new Dictionary<Type, List<EventInfo>>();

        private readonly EventHandler _parentHandler;

        private readonly IEventHandlerHolder _holder;

        /// <summary>
        /// Only for the top-level EventHandlerHolder, which is Game class
        /// </summary>
        public EventHandler(IEventHandlerHolder holder)
        {
            _parentHandler = null;
            _holder = holder;
        }

        private EventHandler(EventHandler parentHandler, IEventHandlerHolder holder)
        {
            _parentHandler = parentHandler;
            _holder = holder;
        }

        public EventHandler GetEventHandler(IEventHandlerHolder newHolder)
        {
            if (_holder.HolderType == newHolder.HolderType)
                throw new InvalidOperationException("You can't make a hierarchy between same type!");

            // return new EventHandler, setting this instance as parent
            return new EventHandler(this, newHolder);
        }

        public void Subscribe<T>(Action<Event, IEventHandlerHolder> c, bool once = false) where T : Event
        {
            var type = typeof(T);
            var eventInfo = new EventInfo(c, once);

            // Make new list and add it when there are no initial list for the given type
            if (!_subscribeInfoDict.ContainsKey(type))
            {
                var list = new List<EventInfo>();
                _subscribeInfoDict.Add(type, list);
            }

            _subscribeInfoDict[type].Add(eventInfo);
        }

        public void UnSubscribe<T>(Action<Event> c) where T : Event
        {
            var type = typeof(T);

            if (!_subscribeInfoDict.TryGetValue(type, out var infos)) return;

            var idx = infos.FindIndex(i => i.Callback.Equals(c));
            if (idx == -1) return;

            infos.RemoveAt(idx);
            if (infos.Count == 0)
                _subscribeInfoDict.Remove(type);
        }

        public void Publish<T>(T e, IEventHandlerHolder holder = null) where T : Event
        {
            var type = typeof(T);
            var removeList = new List<EventInfo>();

            if (!_subscribeInfoDict.TryGetValue(type, out var infos)) return;

            foreach (var i in infos)
            {
                i.Callback.Invoke(e, null);

                if (i.ReceiveOnlyOnce)
                    removeList.Add(i);
            }

            foreach (var i in removeList)
                infos.Remove(i);

            if (infos.Count == 0)
                _subscribeInfoDict.Remove(type);

            _parentHandler?.Publish(e, _holder);

            e.Dispose();
        }
    }
}