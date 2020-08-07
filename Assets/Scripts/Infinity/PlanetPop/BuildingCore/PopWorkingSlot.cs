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

        public readonly string Group;

        public readonly float Wage;

        private readonly Dictionary<string, float> _baseYield;

        public IReadOnlyDictionary<string, float> BaseYield => _baseYield;

        private readonly Dictionary<string, float> _baseUpkeep;

        public IReadOnlyDictionary<string, float> BaseUpkeep => _baseUpkeep;

        public int YieldMultiplier { get; private set; }

        public int UpkeepMultiplier { get; private set; }

        private readonly Neuron _buildingNeuron;

        public Pop Pop { get; private set; }

        private readonly List<GameFactorChange> _yield = new List<GameFactorChange>();

        public IReadOnlyList<GameFactorChange> Yield => _yield;

        private readonly List<GameFactorChange> _upkeep = new List<GameFactorChange>();

        public IReadOnlyList<GameFactorChange> Upkeep => _upkeep;

        public PopWorkingSlot(Neuron buildingNeuron, PopSlotPrototype prototype)
        {
            _buildingNeuron = buildingNeuron;

            _buildingNeuron.Subscribe<PopSlotAssignedSignal>(OnPopSlotAssignedSignal);

            Name = prototype.Name;
            Group = prototype.Group;
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

        private void OnPopSlotAssignedSignal(ISignal s)
        {
            if (!(s is PopSlotAssignedSignal psas)) return;

            if (this != psas.AssignedSlot) return;
            Pop = psas.Pop;
        }
    }

    public class PopSlotStateChangeSignal : ISignal
    {
        public ISignalDispatcherHolder SignalSender { get; }

        public readonly PopWorkingSlot Slot;

        public readonly IReadOnlyDictionary<string, float> YieldDiff, UpkeepDiff;

        public PopSlotStateChangeSignal(ISignalDispatcherHolder sender, PopWorkingSlot slot,
            IReadOnlyDictionary<string, float> yieldDiff, IReadOnlyDictionary<string, float> upkeepDiff)
        {
            SignalSender = sender;
            Slot = slot;
            YieldDiff = yieldDiff;
            UpkeepDiff = upkeepDiff;
        }
    }
}
