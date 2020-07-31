namespace Infinity.GameData
{
    public class BuildingPrototypeData : IGameData
    {
        private string _strAssetDir => UnityEngine.Application.streamingAssetsPath;

        public string DataName => nameof(BuildingPrototypeData);

        public bool Load()
        {
            throw new System.NotImplementedException();
        }
    }
}