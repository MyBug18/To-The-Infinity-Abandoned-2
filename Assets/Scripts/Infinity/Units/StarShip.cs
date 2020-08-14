using System.Collections.Generic;

namespace Infinity.Units
{
    public class StarShip
    {
        public StarShipType ShipType { get; }

        private readonly Dictionary<string, float> _maxCarriableResource = new Dictionary<string, float>();

        public IReadOnlyDictionary<string, float> MaxCarriableResource => _maxCarriableResource;

        private readonly Dictionary<string, float> _currentCarryingResource = new Dictionary<string, float>();

        public IReadOnlyDictionary<string, float> CurrentCarryingResource => _currentCarryingResource;

        private readonly Dictionary<PowerType, float> _powers = new Dictionary<PowerType, float>();

        public IReadOnlyDictionary<PowerType, float> Powers => _powers;
    }

    public enum StarShipType
    {
        Striker,
        Bomber,
        SupplyShip,
        ScienceShip,
    }

    public enum PowerType
    {
        Attack,
        Bombard,
        Science,
    }

}