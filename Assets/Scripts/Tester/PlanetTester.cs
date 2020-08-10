using Infinity;
using Infinity.GameData;
using Infinity.HexTileMap;
using Infinity.PlanetPop;
using UnityEngine;

namespace Tester
{
    public class PlanetTester : MonoBehaviour
    {
        [SerializeField]
        private TileMapWrapper t;

        private void Start()
        {
            var map = Instantiate(t, transform);
            map.Init(new Planet());
        }

        private void TestJson()
        {
            GameDataStorage.Instance.InitializeGameDataManually();

            var data = GameDataStorage.Instance.GetGameData<BuildingData>();

            Debug.Log(data["TestBuilding"].CheckTileState(new HexTile(new HexTileCoord(1, 3), null)));
        }
    }
}
