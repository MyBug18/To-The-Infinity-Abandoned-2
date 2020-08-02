using Infinity;
using Infinity.GameData;
using Infinity.HexTileMap;
using UnityEngine;

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
            GameDataStorage.Instance.InitializeGameDataManually();

            var data = GameDataStorage.Instance.GetGameData<BuildingData>();

            Debug.Log(data["TestBuilding"].CheckTileState(new HexTile(new HexTileCoord(1, 3))));
        }
    }
}
