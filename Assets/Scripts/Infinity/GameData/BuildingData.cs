using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Infinity.GameData
{
    public class BuildingData : IGameData
    {
        public string DataName => nameof(BuildingData);

        private readonly string _dataPath = Path.Combine(Application.streamingAssetsPath, "GameData", "BuildingData");

        private readonly Dictionary<string, BuildingPrototype> _prototypeDict = new Dictionary<string, BuildingPrototype>();

        private readonly Dictionary<string, bool> _availableDict = new Dictionary<string, bool>();

        public IReadOnlyList<string> AvailableBuildingList =>
            _prototypeDict.Keys.Where(s => _availableDict[s]).ToList();

        private Game _game;

        public BuildingPrototype this[string key] => !_prototypeDict.TryGetValue(key, out var result) ? null : result;

        public BuildingData(GameInitializedEventSender sender)
        {
            sender.Subscribe(OnGameInitialized);
        }

        private void OnGameInitialized(Game game)
        {
            _game = game;

            foreach (var name in game.AvailableBuildings)
                _availableDict[name] = true;

            // Should receive events which enables building
        }

        public void Load()
        {
            foreach (var path in Directory.GetFiles(_dataPath))
            {
#if UNITY_EDITOR
                if (!path.EndsWith(".json")) continue;
#endif
                var jsonData = File.ReadAllText(path);

                var building = new BuildingPrototype(jsonData);

                if (_prototypeDict.ContainsKey(building.Name))
                {
                    // should log or throw exception
                    continue;
                }

                _prototypeDict[building.Name] = building;

                // First set all false and wait for the game to be initialized
                _availableDict[building.Name] = false;
            }
        }
    }
}
