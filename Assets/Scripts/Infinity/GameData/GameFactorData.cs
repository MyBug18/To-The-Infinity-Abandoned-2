using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Infinity.GameData
{
    public enum GameFactorKind
    {
        PlanetaryResource,
        GlobalResource,
        PlanetaryFactor,
        GlobalFactor,
    }

    public class GameFactorData : IGameData
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

        public GameFactorData(GameInitializedEventSender sender)
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

        public GameFactorKind GetFactorKind(string GameFactor)
        {
            if (_planetaryResourceDict.ContainsKey(GameFactor))
                return GameFactorKind.PlanetaryResource;

            if (_globalResourceDict.ContainsKey(GameFactor))
                return GameFactorKind.GlobalResource;

            return GameFactorKind.PlanetaryFactor;
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
