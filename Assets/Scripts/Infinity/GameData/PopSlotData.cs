using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Infinity.GameData
{
    public class PopSlotData : IGameData
    {
        public string DataName => nameof(PopSlotData);

        private readonly string _dataPath = Path.Combine(Application.streamingAssetsPath, "GameData", "PopSlotData");

        private readonly Dictionary<string, PopSlotPrototype> _prototypeDict =
            new Dictionary<string, PopSlotPrototype>();

        private readonly Dictionary<string, List<string>> _groupJobDict = new Dictionary<string, List<string>>();

        public PopSlotPrototype this[string key] => !_prototypeDict.TryGetValue(key, out var result) ? null : result;

        public void Load()
        {
            foreach (var path in Directory.GetFiles(_dataPath))
            {
#if UNITY_EDITOR
                if (!path.EndsWith(".json")) continue;
#endif
                var jsonData = File.ReadAllText(path);

                var popSlot = new PopSlotPrototype(jsonData);

                if (_prototypeDict.ContainsKey(popSlot.Name))
                {
                    // should log or throw exception
                    continue;
                }

                _prototypeDict[popSlot.Name] = popSlot;

                if (_groupJobDict.TryGetValue(popSlot.Group, out var list))
                    list.Add(popSlot.Name);
                else
                {
                    _groupJobDict.Add(popSlot.Group, new List<string>());
                    _groupJobDict[popSlot.Group].Add(popSlot.Name);
                }
            }
        }

        public string GetGroupFromJob(string name)
        {
            if (!_prototypeDict.TryGetValue(name, out var prototype))
                throw new InvalidOperationException();

            return prototype.Group;
        }
    }
}
