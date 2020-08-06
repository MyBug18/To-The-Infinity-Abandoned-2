﻿using System;
using System.Collections.Generic;
using Infinity.GameData;

namespace Infinity
{
    public enum SignalDirection
    {
        Local,
        Upward,
        Downward,
    }

    public interface ISignal
    {
        /// <summary>
        /// Who had sent this signal?
        /// </summary>
        ISignalDispatcherHolder SignalSender { get; }
    }

    public interface ISignalDispatcherHolder
    {
        Type HolderType { get; }

        SignalDispatcher SignalDispatcher { get; }
    }

    /// <summary>
    /// Wrapper class of Neuron, without sending signal
    /// </summary>
    public class SignalDispatcher
    {
        private readonly Neuron _neuron;

        public SignalDispatcher(Neuron neuron)
        {
            _neuron = neuron;
        }

        public void Subscribe<T>(Action<ISignal> c) where T : ISignal
        {
            _neuron.Subscribe<T>(c);
        }

        public void UnSubscribe<T>(Action<ISignal> c) where T : ISignal
        {
            _neuron.UnSubscribe<T>(c);
        }
    }

    public class Neuron
    {
        private readonly Dictionary<Type, List<Action<ISignal>>> _subscribeInfoDict = new Dictionary<Type, List<Action<ISignal>>>();

        private readonly Neuron _parentNeuron;
        private readonly List<Neuron> _childNeurons = new List<Neuron>();

        private readonly ISignalDispatcherHolder _holder;

        public readonly EventConditionPasser EventConditionPasser;

        private Neuron(ISignalDispatcherHolder holder)
        {
            _holder = holder;
            _parentNeuron = null;
            EventConditionPasser = new EventConditionPasser(_childNeurons);
        }

        /// <summary>
        /// Only for the top-level Neuron holder, which is Game class
        /// </summary>
        public static Neuron GetNeuronForGame(ISignalDispatcherHolder holder) => new Neuron(holder);

        private Neuron(ISignalDispatcherHolder holder, Neuron parentNeuron)
        {
            _holder = holder;
            _parentNeuron = parentNeuron;
            EventConditionPasser = new EventConditionPasser(_childNeurons);
        }

        public Neuron GetChildNeuron(ISignalDispatcherHolder childHolder)
        {
            var newNeuron = new Neuron(childHolder, this);
            _childNeurons.Add(newNeuron);
            return newNeuron;
        }

        public void Subscribe<T>(Action<ISignal> callBack) where T : ISignal
        {
            var type = typeof(T);

            // Make new list and add it when there are no initial list for the given type
            if (!_subscribeInfoDict.ContainsKey(type))
            {
                var list = new List<Action<ISignal>>();
                _subscribeInfoDict.Add(type, list);
            }

            _subscribeInfoDict[type].Add(callBack);
        }

        public void UnSubscribe<T>(Action<ISignal> callBack) where T : ISignal
        {
            var type = typeof(T);

            if (!_subscribeInfoDict.TryGetValue(type, out var infos)) return;

            infos.Remove(callBack);

            if (infos.Count == 0)
                _subscribeInfoDict.Remove(type);
        }

        public void SendSignal<T>(T signal, SignalDirection direction) where T : ISignal
        {
            var type = typeof(T);

            if (_subscribeInfoDict.TryGetValue(type, out var infos))
                foreach (var callBack in infos)
                    callBack.Invoke(signal);

            switch (direction)
            {
                case SignalDirection.Upward:
                    _parentNeuron?.SendSignal(signal, direction);
                    break;
                case SignalDirection.Downward:
                    foreach (var n in _childNeurons)
                        n.SendSignal(signal, direction);
                    break;
                case SignalDirection.Local:
                    // do nothing
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public void RemoveChild(ISignalDispatcherHolder holder)
        {
            _childNeurons.RemoveAll(x => x._holder == holder);
        }
    }

    public class EventConditionPasser
    {
        private readonly List<Neuron> _children;

        private Func<List<PassiveEventPrototype>, List<PassiveEventPrototype>> _eventListRefiner;

        public EventConditionPasser(List<Neuron> children)
        {
            _children = children;
        }

        public void OnPassedEventList(List<PassiveEventPrototype> events)
        {
            if (events == null || events.Count == 0) return;

            var newEvents = _eventListRefiner != null ? _eventListRefiner(events) : events;

            foreach (var n in _children)
                n.EventConditionPasser.OnPassedEventList(newEvents);
        }

        public void SetRefiner(Func<List<PassiveEventPrototype>, List<PassiveEventPrototype>> eventListRefiner)
        {
            _eventListRefiner = eventListRefiner;
        }
    }
}
