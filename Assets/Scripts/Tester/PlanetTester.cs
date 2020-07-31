using System.Collections.Generic;
using System.IO;
using Infinity.GameData;
using Infinity.PlanetPop;
using UnityEngine;
using Newtonsoft.Json;
using Infinity;
using System;

namespace Tester
{

    public class PlanetTester : MonoBehaviour
    {
        [SerializeField]
        private GameObject cube;

        private void Start()
        {
            File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "asdf.txt"), "ASDFASDFQASDFADSFADSF\n");

            var n = 10;

            Func<int, bool> f = i => i > n;

            File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "result.txt"), $"ASDFASDF {f(12)}");

            cube.SetActive(false);
        }

        private void TestJson()
        {
            GameDataStorage.Instance.InitializeManually();

            var data = GameDataStorage.Instance.GetGameData<BuildingData>();

            File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "test.json"), JsonConvert.SerializeObject(data["TestBuilding"], Formatting.Indented));
        }
    }
}