using Infinity.GalaxySystem;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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

        [JsonIgnore]
        public SignalDispatcher SignalDispatcher { get; }

        [JsonIgnore]
        public readonly Galaxy Galaxy;

        private readonly List<string> _availableBuildings = new List<string>();

        public IReadOnlyList<string> AvailableBuildings => _availableBuildings;

        private readonly List<string> _availableResources = new List<string>();

        public IReadOnlyList<string> AvailableResources => _availableResources;

        public Game(string dataPath)
        {
            _neuron = Neuron.GetNeuronForGame(this);

            SignalDispatcher = new SignalDispatcher(_neuron);

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
            _neuron.SendSignal(new GameCommandSignal(this, GameCommandType.StartNewTurn), SignalDirection.Downward);
            MonthsPassed += GameSpeed;
        }
    }

    /// <summary>
    /// This signal will be always sent from Game through all neurons
    /// </summary>
    public class GameCommandSignal : ISignal
    {
        public ISignalDispatcherHolder SignalSender { get; }

        public readonly GameCommandType CommandType;

        public GameCommandSignal(ISignalDispatcherHolder holder, GameCommandType type)
        {
            SignalSender = holder;
            CommandType = type;
        }
    }

    public enum GameCommandType
    {
        StartNewTurn,
        CheckEventCondition,
    }
}
