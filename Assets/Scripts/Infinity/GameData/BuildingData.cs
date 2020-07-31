using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Infinity.GameData
{
    public class BuildingData : IGameData
    {
        public string DataName => nameof(BuildingData);

        private readonly string dataPath = Path.Combine(Application.streamingAssetsPath, "GameData", "BuildingData");

        private Dictionary<string, BuildingPrototype> _dict = new Dictionary<string, BuildingPrototype>();

        public BuildingPrototype this[string key]
        {
            get
            {
                if (!_dict.TryGetValue(key, out var result))
                {
                    // should log or throw exception
                    return null;
                }

                return result;
            }
        }

        public void Load()
        {
            foreach (var path in Directory.GetFiles(dataPath))
            {
                if (!path.EndsWith(".json")) continue;

                var jsonData = File.ReadAllText(path);

                var building = new BuildingPrototype(jsonData);

                if (_dict.ContainsKey(building.Name))
                {
                    // should log or throw exception
                    continue;
                }

                _dict[building.Name] = building;
            }
        }
    }
}