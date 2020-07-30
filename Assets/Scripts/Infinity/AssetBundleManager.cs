namespace Infinity
{
    public class AssetBundleManager
    {
        private static AssetBundleManager _instance;

        public static AssetBundleManager Instance => _instance ??= new AssetBundleManager();
    }
}