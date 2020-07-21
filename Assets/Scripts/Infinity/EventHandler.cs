using System;
using System.Collections.Generic;

namespace Infinity
{
    public abstract class Event : IDisposable
    {
        public IEventHandlerHolder Sender { get; private set; }

        public Event(IEventHandlerHolder holder)
        {
            Sender = holder;
        }

        public virtual void Dispose()
        {
            Sender = null;
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
        private readonly Dictionary<Type, List<Action<Event>>> _subscribeInfoDict = new Dictionary<Type, List<Action<Event>>>();

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

        public void Subscribe<T>(Action<Event> callBack) where T : Event
        {
            var type = typeof(T);

            // Make new list and add it when there are no initial list for the given type
            if (!_subscribeInfoDict.ContainsKey(type))
            {
                var list = new List<Action<Event>>();
                _subscribeInfoDict.Add(type, list);
            }

            _subscribeInfoDict[type].Add(callBack);
        }

        public void UnSubscribe<T>(Action<Event> callBack) where T : Event
        {
            var type = typeof(T);

            if (!_subscribeInfoDict.TryGetValue(type, out var infos)) return;

            infos.Remove(callBack);

            if (infos.Count == 0)
                _subscribeInfoDict.Remove(type);
        }

        public void Publish<T>(T e) where T : Event
        {
            var type = typeof(T);

            if (!_subscribeInfoDict.TryGetValue(type, out var infos)) return;

            foreach (var callBack in infos)
                callBack.Invoke(e);

            _parentHandler?.Publish(e);

            e.Dispose();
        }
    }
}