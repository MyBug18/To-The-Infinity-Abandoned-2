﻿using Infinity.GalaxySystem;
using System;

namespace Infinity
{
    public enum GameSpeed
    {
        Slow = 1,
        Normal = 2,
        Fast = 3,
    }

    public class Game : ISignalDispatcherHolder
    {
        /// <summary>
        /// How many month pass when a turn goes on
        /// </summary>
        public int GameSpeed { get; private set; } = 2;

        /// <summary>
        /// How many Months has passed
        /// </summary>
        public int MonthsPassed { get; private set; }

        private readonly Neuron _neuron;

        Type ISignalDispatcherHolder.HolderType => typeof(Game);

        public SignalDispatcher SignalDispatcher { get; }

        public readonly Galaxy Galaxy;

        public Game()
        {
            _neuron = Neuron.GetNeuronForGame(this);
            SignalDispatcher = new SignalDispatcher(_neuron);

            _neuron.Subscribe<GameEventSignal<Game>>(OnGameEventSignal);

            Galaxy = new Galaxy(_neuron);
        }

        /// <summary>
        /// End this turn and starts an enemy turn
        /// </summary>
        public void EndTurn()
        {
        }

        /// <summary>
        /// Start new turn
        /// </summary>
        public void StartNewTurn()
        {
            _neuron.SendSignal(new NextTurnSignal(this), SignalDirection.Downward);
            MonthsPassed += GameSpeed;
        }

        private void OnGameEventSignal(ISignal s)
        {
            if (!(s is GameEventSignal<Game> ges)) return;
        }
    }

    public class NextTurnSignal : ISignal
    {
        public ISignalDispatcherHolder SignalSender { get; }

        public NextTurnSignal(ISignalDispatcherHolder holder)
        {
            SignalSender = holder;
        }
    }
}
