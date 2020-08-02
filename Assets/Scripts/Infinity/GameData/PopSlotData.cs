﻿using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Infinity.GameData
{
    public class PopSlotData : IGameData
    {
        public string DataName => nameof(PopSlotData);

        private readonly string dataPath = Path.Combine(Application.streamingAssetsPath, "GameData", "PopSlotData");

        private readonly Dictionary<string, PopSlotPrototype> _dict = new Dictionary<string, PopSlotPrototype>();

        private Game _game;

        public PopSlotPrototype this[string key]
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

        public PopSlotData(GameInitializedEventSender sender)
        {
            sender.Subscribe(OnGameInitialized);
        }

        private void OnGameInitialized(Game game)
        {
            _game = game;
        }

        public void Load()
        {
            foreach (var path in Directory.GetFiles(dataPath))
            {
                if (!path.EndsWith(".json")) continue;

                var jsonData = File.ReadAllText(path);

                var popSlot = new PopSlotPrototype(jsonData);

                if (_dict.ContainsKey(popSlot.Name))
                {
                    // should log or throw exception
                    continue;
                }

                _dict[popSlot.Name] = popSlot;
            }
        }
    }
}
