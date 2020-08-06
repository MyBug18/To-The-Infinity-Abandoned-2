using Infinity.GalaxySystem;
using Newtonsoft.Json;
using System;
using System.Collections;
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

        private readonly Neuron _neuron;

        Type ISignalDispatcherHolder.HolderType => typeof(Game);

        [JsonIgnore]
        public SignalDispatcher SignalDispatcher { get; }

        private readonly TileMap _tileMap;

        public int TileMapRadius => _tileMap.Radius;

        public TileMapType TileMapType => TileMapType.Game;

        public IReadOnlyList<IOnHexTileObject> this[HexTileCoord coord] => _tileMap[coord];

        private readonly List<string> _availableBuildings = new List<string>();

        public IReadOnlyList<string> AvailableBuildings => _availableBuildings;

        private readonly List<string> _availableResources = new List<string>();

        public IReadOnlyList<string> AvailableResources => _availableResources;

        public Game(string dataPath)
        {
            _neuron = Neuron.GetNeuronForGame(this);
            SignalDispatcher = new SignalDispatcher(_neuron);
            _neuron.EventConditionPasser.SetRefiner(OnPassiveEventCheck);

            _tileMap = new TileMap(6, _neuron);
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
            //TODO: Check events
            MonthsPassed += GameSpeed;
        }

        private List<PassiveEventPrototype> OnPassiveEventCheck(List<PassiveEventPrototype> events)
        {
            var passed = new List<PassiveEventPrototype>();

            foreach (var prototype in events.Where(prototype =>
                prototype.GameConditionChecker.Evaluate(this)))
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

        public bool IsValidCoord(HexTileCoord coord) => _tileMap.IsValidCoord(coord);

        public HexTile GetHexTile(HexTileCoord coord) => _tileMap.GetHexTile(coord);

        public T GetTileObject<T>(HexTileCoord coord) where T : IOnHexTileObject =>
            _tileMap.GetTileObject<T>(coord);

        public IReadOnlyList<T> GetTileObjectList<T>() where T : IOnHexTileObject =>
            _tileMap.GetTileObjectList<T>();

        public IEnumerator<HexTile> GetEnumerator()
        {
            return _tileMap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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
