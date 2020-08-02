using Infinity.GameData;
using System;
using System.Collections.Generic;

namespace Infinity
{
    public class GameInitializedEventSender
    {
        private readonly List<Action<Game>> _eventReceivers = new List<Action<Game>>();

        public void SendInitializedEvent(Game game)
        {
            foreach (var a in _eventReceivers)
                a.Invoke(game);
        }

        public void Subscribe(Action<Game> action)
        {
            _eventReceivers.Add(action);
        }
    }

    /// <summary>
    /// Will store every game data, including game itself
    /// </summary>
    public class GameDataStorage
    {
        private static GameDataStorage _instance;

        public static GameDataStorage Instance => _instance == null ? (_instance = new GameDataStorage()) : _instance;

        private readonly Dictionary<Type, IGameData> _gameDataDict = new Dictionary<Type, IGameData>();

        private readonly GameInitializedEventSender _gameInitializedSender = new GameInitializedEventSender();

        private void InitializeGameData()
        {
            _gameDataDict[typeof(BuildingData)] = new BuildingData(_gameInitializedSender);
            _gameDataDict[typeof(PopSlotData)] = new PopSlotData(_gameInitializedSender);

            foreach (var data in _gameDataDict.Values)
                data.Load();
        }

        /// <summary>
        /// For test
        /// </summary>
        public void InitializeGameDataManually()
        {
            InitializeGameData();
        }

        public T GetGameData<T>() where T : IGameData
        {
            if (!_gameDataDict.TryGetValue(typeof(T), out var gameData))
                throw new InvalidOperationException($"There are no GameData: {nameof(T)}");

            return (T)gameData;
        }
    }
}
