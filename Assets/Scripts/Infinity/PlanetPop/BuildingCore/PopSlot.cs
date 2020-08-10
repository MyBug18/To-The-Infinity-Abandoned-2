using System.Collections.Generic;
using Infinity.GameData;
using Infinity.Modifiers;

namespace Infinity.PlanetPop.BuildingCore
{
    public enum PopSlotState
    {
        Empty,
        Occupied,
        TrainingForHere,
    }

    public class PopSlot
    {
        public PopSlotState CurrentState { get; private set; } = PopSlotState.Empty;

        public readonly string Name;

        public readonly string Group;

        public readonly float Wage;

        private readonly Dictionary<string, float> _baseYield;

        public IReadOnlyDictionary<string, float> BaseYield => _baseYield;

        private readonly Dictionary<string, float> _baseUpkeep;

        public IReadOnlyDictionary<string, float> BaseUpkeep => _baseUpkeep;

        private Dictionary<string, int> _yieldMultiplier = new Dictionary<string, int>();

        private Dictionary<string, int> _upkeepMultiplier = new Dictionary<string, int>();

        private readonly Neuron _buildingNeuron;

        public Pop Pop { get; private set; }

        private readonly List<GameFactorChange> _yield = new List<GameFactorChange>();

        public IReadOnlyList<GameFactorChange> Yield => _yield;

        private readonly List<GameFactorChange> _upkeep = new List<GameFactorChange>();

        public IReadOnlyList<GameFactorChange> Upkeep => _upkeep;

        public PopSlot(Neuron buildingNeuron, PopSlotPrototype prototype, IReadOnlyList<Modifier> modifiers = null)
        {
            _buildingNeuron = buildingNeuron;

            _buildingNeuron.Subscribe<PopSlotAssignedSignal>(OnPopSlotAssignedSignal);

            Name = prototype.Name;
            Group = prototype.Group;
            Wage = prototype.Wage;

            if (modifiers != null)
                foreach (var m in modifiers)
                {
                    var mDict = m.ModifierInfo.GameFactorMultiplier;

                    foreach (var kv in mDict)
                    {
                        if (!_yieldMultiplier.ContainsKey(kv.Key))
                            _yieldMultiplier.Add(kv.Key, 0);

                        _yieldMultiplier[kv.Key] += kv.Value;
                    }
                }

            foreach (var y in prototype.Yield)
            {
                _baseYield.Add(y.FactorType, y.Amount);

                float YieldGetter()
                {
                    if (!_yieldMultiplier.TryGetValue(y.FactorType, out var yieldMultiplier))
                        yieldMultiplier = 0;

                    if (_yieldMultiplier.TryGetValue("AnyResource", out var anyResource))
                        yieldMultiplier += anyResource;

                    return CurrentState == PopSlotState.Occupied
                        ? _baseYield[y.FactorType] * (1 + yieldMultiplier / 100f) * (1 + Pop.YieldMultiplier / 100f)
                        : 0;
                }

                _yield.Add(new GameFactorChange(YieldGetter, y.FactorType));
            }

            foreach (var u in prototype.Upkeep)
            {
                _baseUpkeep.Add(u.FactorType, u.Amount);

                float UpkeepGetter()
                {
                    if (!_yieldMultiplier.TryGetValue(u.FactorType, out var upkeepMultiplier))
                        upkeepMultiplier = 0;

                    if (_yieldMultiplier.TryGetValue("AnyResource", out var anyResource))
                        upkeepMultiplier += anyResource;

                    return CurrentState == PopSlotState.Occupied
                        ? _baseUpkeep[u.FactorType] * (1 + upkeepMultiplier / 100f) * (1 + Pop.UpkeepMultiplier / 100f)
                        : 0;
                }

                _upkeep.Add(new GameFactorChange(UpkeepGetter, u.FactorType));
            }
        }

        private void AddModifier(Modifier m)
        {

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

        public readonly PopSlot Slot;

        public readonly IReadOnlyDictionary<string, float> YieldDiff, UpkeepDiff;

        public PopSlotStateChangeSignal(ISignalDispatcherHolder sender, PopSlot slot,
            IReadOnlyDictionary<string, float> yieldDiff, IReadOnlyDictionary<string, float> upkeepDiff)
        {
            SignalSender = sender;
            Slot = slot;
            YieldDiff = yieldDiff;
            UpkeepDiff = upkeepDiff;
        }
    }
}
