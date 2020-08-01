using System.Collections.Generic;
using System.IO;
using Infinity.GameData;
using Infinity.PlanetPop;
using UnityEngine;
using Newtonsoft.Json;
using Infinity;
using System;
using Infinity.HexTileMap;

namespace Tester
{

    public class PlanetTester : MonoBehaviour
    {
        [SerializeField]
        private GameObject cube;

        private void Start()
        {
            TestJson();

            cube.SetActive(false);
        }

        private void TestJson()
        {
            GameDataStorage.Instance.InitializeManually();

            var data = GameDataStorage.Instance.GetGameData<BuildingData>();

            Debug.Log(data["TestBuilding"].CheckTileState(new HexTile(new HexTileCoord(1, 3))));

 //           File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "test.json"), JsonConvert.SerializeObject(data["TestBuilding"], Formatting.Indented));
        }
    }
}