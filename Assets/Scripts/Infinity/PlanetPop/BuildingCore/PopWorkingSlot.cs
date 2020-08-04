using System;
using System.Collections.Generic;
using Infinity.GameData;

namespace Infinity.PlanetPop.BuildingCore
{
    public enum PopSlotState
    {
        Empty,
        Occupied,
        TrainingForHere,
    }

    public class PopWorkingSlot
    {
        public PopSlotState CurrentState { get; private set; } = PopSlotState.Empty;

        public readonly string Name;

        public readonly float Wage;

        private readonly Dictionary<GameFactorType, float> _baseYield;

        public IReadOnlyDictionary<GameFactorType, float> BaseYield => _baseYield;

        private readonly Dictionary<GameFactorType, float> _baseUpkeep;

        public IReadOnlyDictionary<GameFactorType, float> BaseUpkeep => _baseUpkeep;

        public int YieldMultiplier { get; private set; }

        public int UpkeepMultiplier { get; private set; }

        private readonly Neuron _buildingNeuron;

        public Pop Pop { get; private set; }

        private readonly List<GameFactorChange> _yield = new List<GameFactorChange>();

        private readonly List<GameFactorChange> _upkeep = new List<GameFactorChange>();

        public PopWorkingSlot(Neuron buildingNeuron, PopSlotPrototype prototype)
        {
            _buildingNeuron = buildingNeuron;

            Name = prototype.Name;
            Wage = prototype.Wage;

            foreach (var y in prototype.Yield)
            {
                _baseYield.Add(y.FactorType, y.Amount);

                float YieldGetter()
                {
                    return CurrentState == PopSlotState.Occupied
                        ? _baseYield[y.FactorType] * (1 + YieldMultiplier / 100f) * (1 + Pop.YieldMultiplier / 100f)
                        : 0;
                }

                _yield.Add(new GameFactorChange(YieldGetter, y.FactorType));
            }

            foreach (var u in prototype.Upkeep)
            {
                _baseUpkeep.Add(u.FactorType, u.Amount);

                float UpkeepGetter()
                {
                    return CurrentState == PopSlotState.Occupied
                        ? _baseUpkeep[u.FactorType] * (1 + UpkeepMultiplier / 100f) * (1 + Pop.UpkeepMultiplier / 100f)
                        : 0;
                }

                _upkeep.Add(new GameFactorChange(UpkeepGetter, u.FactorType));
            }
        }
    }
}
