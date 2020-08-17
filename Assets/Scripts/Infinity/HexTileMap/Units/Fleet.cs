using System.Collections.Generic;

namespace Infinity.HexTileMap.Units
{
    public class Fleet : IMovableUnit
    {
        public string Name { get; private set; }

        public HexTileCoord HexCoord { get; private set; }

        public OwnerType OwnerType { get; }

        public UnitState CurrentState { get; }

        private Neuron _currentTileMapNeuron;

        private TileMap _currentTileMap;

        public int FleetCapacity { get; private set; }

        private readonly List<StarShip> _starShips = new List<StarShip>();

        public IReadOnlyList<StarShip> StarShips => _starShips;

        public int MovableRange { get; private set; } = 2;

        public int RemainMovePoint { get; private set; }

        public Fleet(List<StarShip> ships, TileMap tileMap, Neuron tileMapNeuron, HexTileCoord coord)
        {
            _starShips = ships;
            _currentTileMap = tileMap;
            _currentTileMapNeuron = tileMapNeuron;
            HexCoord = coord;
        }

        /// <summary>
        /// Fleets has no restriction in moving
        /// </summary>
        public List<HexTileCoord> GetMovableTiles()
        {
            var result = new List<HexTileCoord>();

            for (var i = 1; i <= MovableRange; i++)
                result.AddRange(_currentTileMap.GetRing(i, HexCoord));

            return result;
        }
    }
}