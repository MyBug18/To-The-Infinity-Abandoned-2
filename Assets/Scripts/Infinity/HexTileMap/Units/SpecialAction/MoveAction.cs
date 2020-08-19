using System;

namespace Infinity.HexTileMap.Units.SpecialAction
{
    public class MoveAction : ISpecialAction
    {
        private readonly IMovableUnit _unit;

        private readonly Action<HexTileCoord> _moveAction;

        public string SpecialActionName => "Move";

        public bool IsVisible() => true;

        public bool IsAvailable(HexTileCoord coord)
        {
            var side = _unit.OwnerType == OwnerType.Me && Game.IsMyTurn ||
                       _unit.OwnerType == OwnerType.Enemy && !Game.IsMyTurn;

            return side && _unit.RemainMovePoint >= _unit.HexCoord.GetDistance(coord);
        }

        public void DoSpecialAction(HexTileCoord coord) => _moveAction(coord);

        public MoveAction(IMovableUnit unit, Action<HexTileCoord> moveAction)
        {
            _unit = unit;
            _moveAction = moveAction;
        }
    }
}