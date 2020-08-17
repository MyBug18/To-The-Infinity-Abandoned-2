using System.Collections.Generic;
using System.Linq;
using Infinity.GameData;
using Infinity.HexTileMap;

namespace Infinity
{
    public enum GameSpeed
    {
        Slow = 1,
        Normal = 2,
        Fast = 3,
    }

    public class Game : ITileMapHolder
    {
        /// <summary>
        /// How many month pass when a turn goes on
        /// </summary>
        public int GameSpeed { get; private set; } = 2;

        /// <summary>
        /// How many Months has passed
        /// </summary>
        public int MonthsPassed { get; private set; }

        public bool IsFTLUnlocked { get; private set; }

        private readonly Neuron _neuron;

        public TileMap TileMap { get; }

        private readonly HashSet<string> _availableBuildings = new HashSet<string>();

        public IReadOnlyCollection<string> AvailableBuildings => _availableBuildings;

        public Game(string dataPath)
        {
            _neuron = Neuron.GetNeuronForGame();
            _neuron.EventConditionPasser.SetRefiner(OnPassiveEventCheck);

            TileMap = new TileMap(6, _neuron);
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
            _neuron.SendSignal(new GameCommandSignal(_neuron, GameCommandType.StartNewTurn), SignalDirection.Downward);
            //TODO: Check events
            MonthsPassed += GameSpeed;
        }

        private List<PassiveEventPrototype> OnPassiveEventCheck(List<PassiveEventPrototype> events)
        {
            var passed = new List<PassiveEventPrototype>();

            foreach (var prototype in events.Where(prototype => prototype.GameConditionChecker?.Invoke(this) ?? true))
            {
                if (prototype.EventOwnerType == "Game" && Utils.GetBoolFromChance(prototype.Chance))
                {
                    // occur event and send signal
                }
                else
                {
                    passed.Add(prototype);
                }
            }

            return passed;
        }
    }

    /// <summary>
    /// This signal will be always sent from Game through all neurons
    /// </summary>
    public class GameCommandSignal : ISignal
    {
        public Neuron FromNeuron { get; }

        public readonly GameCommandType CommandType;

        public GameCommandSignal(Neuron neuron, GameCommandType type)
        {
            FromNeuron = neuron;
            CommandType = type;
        }
    }

    public enum GameCommandType
    {
        StartNewTurn,
        CheckEventCondition,
    }
}
