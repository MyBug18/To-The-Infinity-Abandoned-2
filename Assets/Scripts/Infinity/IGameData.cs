namespace Infinity
{
    public interface IGameData
    {
        string DataName { get; }

        bool Load();
    }
}