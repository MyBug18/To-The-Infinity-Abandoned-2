using System.Collections.Generic;

namespace Infinity.HexTileMap
{
    public interface IMovableUnit : IOnHexTileObject
    {
        UnitState CurrentState { get; }

        void Move(HexTileCoord destination);

        List<HexTileCoord> GetMovableTiles();
    }

    public enum UnitState
    {
        Idle,
        Defensive,
        InBattle
    }
}