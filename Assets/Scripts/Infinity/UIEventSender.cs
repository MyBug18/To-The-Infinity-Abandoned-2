using System;
using System.Collections.Generic;

namespace Infinity
{
    public abstract class Event : IDisposable
    {
        public IEventSenderHolder Sender { get; private set; }

        public Event(IEventSenderHolder holder)
        {
            Sender = holder;
        }

        public virtual void Dispose()
        {
            Sender = null;
        }
    }

    public interface IEventSenderHolder
    {
        Type HolderType { get; }
        UIEventSender UIEventSender { get; }
    }

    /// <summary>
    /// Super simple event handler with hierarchy
    /// </summary>
    public class UIEventSender
    {
        private readonly Dictionary<Type, List<Action<Event>>> _subscribeInfoDict = new Dictionary<Type, List<Action<Event>>>();

        private readonly UIEventSender _parentSender;

        private readonly IEventSenderHolder _holder;

        /// <summary>
        /// Only for the top-level EventHandlerHolder, which is Game class
        /// </summary>
        public UIEventSender(IEventSenderHolder holder)
        {
            _parentSender = null;
            _holder = holder;
        }

        private UIEventSender(UIEventSender parentSender, IEventSenderHolder holder)
        {
            _parentSender = parentSender;
            _holder = holder;
        }

        public UIEventSender GetUIEventsender(IEventSenderHolder newHolder)
        {
            if (_holder.HolderType == newHolder.HolderType)
                throw new InvalidOperationException("You can't make a hierarchy between same type!");

            // return new EventHandler, setting this instance as parent
            return new UIEventSender(this, newHolder);
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

        public void SendEvent<T>(T e) where T : Event
        {
            var type = typeof(T);

            if (!_subscribeInfoDict.TryGetValue(type, out var infos)) return;

            foreach (var callBack in infos)
                callBack.Invoke(e);

            _parentSender?.SendEvent(e);

            e.Dispose();
        }
    }
}