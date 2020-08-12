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
        ResearchResource,
        PlanetaryFactor,
        GlobalFactor,
    }

    public class GameFactorResourceData : IGameData
    {
        private readonly string _dataPath =
            Path.Combine(Application.streamingAssetsPath, "GameData", "BuildingData", "ResourceData.json");

        private readonly HashSet<string> _planetaryResourceSet = new HashSet<string>();

        public IReadOnlyCollection<string> PlanetaryResourceSet => _planetaryResourceSet;

        private readonly HashSet<string> _globalResourceSet = new HashSet<string>();

        public IReadOnlyCollection<string> GlobalResourceSet => _globalResourceSet;

        private readonly HashSet<string> _researchResourceSet = new HashSet<string>();

        public IReadOnlyCollection<string> ResearchResourceSet => _researchResourceSet;

        private readonly HashSet<string> _planetaryGameFactor = new HashSet<string>
        {
            "PopGrowth",
            "Amenity",
            "Crime",
        };

        private readonly HashSet<string> _globalGameFactor = new HashSet<string>
        {
            "XenophobeOpinion",
            "XenophileOpinion",
        };

        public IReadOnlyCollection<string> AllResourceSet
        {
            get
            {
                var result = new HashSet<string>();

                foreach (var s in _planetaryResourceSet)
                    result.Add(s);

                foreach (var s in _globalResourceSet)
                    result.Add(s);

                foreach (var s in _researchResourceSet)
                    result.Add(s);

                return result;
            }
        }

        public GameFactorKind GetFactorKind(string GameFactor)
        {
            if (_planetaryResourceSet.Contains(GameFactor))
                return GameFactorKind.PlanetaryResource;

            if (_globalResourceSet.Contains(GameFactor))
                return GameFactorKind.GlobalResource;

            if (_researchResourceSet.Contains(GameFactor))
                return GameFactorKind.ResearchResource;

            if (_planetaryGameFactor.Contains(GameFactor))
                return GameFactorKind.PlanetaryFactor;

            if (_globalGameFactor.Contains(GameFactor))
                return GameFactorKind.GlobalFactor;

            throw new InvalidOperationException();
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
                _planetaryResourceSet.Add(r);

            foreach (var r in globalResource)
                _globalResourceSet.Add(r);
        }
    }
}
