namespace Infinity.GameData
{
    public class BuildingData : IGameData
    {
        private string _strAssetDir => UnityEngine.Application.streamingAssetsPath;

        public string DataName => nameof(BuildingData);

        public bool Load()
        {
            throw new System.NotImplementedException();
        }
    }
}