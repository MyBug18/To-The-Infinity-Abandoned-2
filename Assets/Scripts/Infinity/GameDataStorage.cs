using System;
using System.Collections.Generic;
using Infinity.GameData;

namespace Infinity
{
    public class GameDataStorage
    {
        private static GameDataStorage _instance;

        public static GameDataStorage Instance => _instance ??= new GameDataStorage();

        private Dictionary<Type, IGameData> _gameDataDict = new Dictionary<Type, IGameData>();

        private void Initialize()
        {
            _gameDataDict[typeof(BuildingData)] = new BuildingData();
        }

        public T GetGameData<T>() where T : IGameData
        {
            if (!_gameDataDict.TryGetValue(typeof(T), out var gameData))
                throw new InvalidOperationException($"There are no GameData: {nameof(T)}");

            return (T) gameData;
        }
    }
}