namespace Infinity
{
    public class GameDataStorage
    {
        private static GameDataStorage _instance;

        public static GameDataStorage Instance => _instance ??= new GameDataStorage();
    }
}