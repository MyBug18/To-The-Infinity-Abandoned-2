namespace Infinity
{
    public interface IGameData
    {
        string DataName { get; }

        void Load();
    }
}
