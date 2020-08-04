using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Infinity.GameData
{
    public class ResourceData : IGameData
    {
        private readonly string _dataPath =
            Path.Combine(Application.streamingAssetsPath, "GameData", "BuildingData", "ResourceData.json");

        private Game _game;

        public string DataName { get; }

        private readonly Dictionary<string, bool> _planetaryResourceDict = new Dictionary<string, bool>();

        private readonly Dictionary<string, bool> _globalResourceDict = new Dictionary<string, bool>();

        public IReadOnlyList<string> AllResourceList
        {
            get
            {
                var result = new List<string>();

                result.AddRange(_planetaryResourceDict.Keys);
                result.AddRange(_globalResourceDict.Keys);

                return result;
            }
        }

        public ResourceData(GameInitializedEventSender sender)
        {
            sender.Subscribe(OnGameInitialized);
        }

        public bool IsResourceAvailable(string resource)
        {
            if (_planetaryResourceDict.TryGetValue(resource, out var result))
                return result;

            if (_globalResourceDict.TryGetValue(resource, out result))
                return result;

            throw new InvalidOperationException();
        }

        private void OnGameInitialized(Game game)
        {
            _game = game;

            foreach (var name in game.AvailableResources)
            {
                if (_planetaryResourceDict.ContainsKey(name))
                {
                    _planetaryResourceDict[name] = true;
                    continue;
                }

                if (!_globalResourceDict.ContainsKey(name))
                    throw new InvalidOperationException();

                _globalResourceDict[name] = true;
            }

            // Should receive events which enables building
        }

        public void Load()
        {
            var jsonData = File.ReadAllText(_dataPath);
            var primary = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);

            var planetResource = JArray.FromObject(primary["PlanetaryResources"]).ToObject<List<string>>();
            var globalResource = JArray.FromObject(primary["GlobalResources"]).ToObject<List<string>>();

            if (planetResource == null || globalResource == null)
                throw new NullReferenceException();

            foreach (var r in planetResource)
                _planetaryResourceDict.Add(r, false);

            foreach (var r in globalResource)
                _globalResourceDict.Add(r, false);
        }
    }
}