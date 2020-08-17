using System.Collections.Generic;

namespace Infinity.HexTileMap
{
    public interface IMovableUnit : IOnHexTileObject
    {
        UnitState CurrentState { get; }

        int RemainMovePoint { get; }

        List<HexTileCoord> GetMovableTiles();
    }

    public enum UnitState
    {
        Idle,
        Defensive,
        InBattle
    }
}