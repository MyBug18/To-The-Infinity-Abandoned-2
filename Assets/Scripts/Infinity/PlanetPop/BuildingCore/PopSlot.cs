using System.Collections.Generic;
using System.Linq;
using Infinity.GameData;

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

        private readonly Dictionary<string, float> _baseYield = new Dictionary<string, float>();

        public IReadOnlyDictionary<string, float> BaseYield => _baseYield;

        private readonly Dictionary<string, float> _baseUpkeep = new Dictionary<string, float>();

        public IReadOnlyDictionary<string, float> BaseUpkeep => _baseUpkeep;

        public Pop Pop { get; private set; }

        private readonly List<GameFactorChange> _yield = new List<GameFactorChange>();

        public IReadOnlyList<GameFactorChange> Yield => _yield;

        private readonly List<GameFactorChange> _upkeep = new List<GameFactorChange>();

        public IReadOnlyList<GameFactorChange> Upkeep => _upkeep;

        public PopSlot(Neuron buildingNeuron, Building building, PopSlotPrototype prototype)
        {
            buildingNeuron.Subscribe<PopSlotAssignedSignal>(OnPopSlotAssignedSignal);
            buildingNeuron.Subscribe<PopTrainingStatusChangeSignal>(OnPopTrainingStatusChangeSignal);

            Name = prototype.Name;
            Group = prototype.Group;

            var resourceData = GameDataStorage.Instance.GetGameData<GameFactorResourceData>();

            var yieldModifierMultiplier = building.JobYieldMultiplierFromModifier;
            var yieldAdjacencyMultiplier = building.AdjacencyBonusLevel * building.AdjacencyBonusPerLevel;

            foreach (var y in prototype.Yield)
            {
                _baseYield.Add(y.FactorType, y.Amount);

                float YieldGetter()
                {
                    if (CurrentState != PopSlotState.Occupied) return 0;

                    // do not apply any multiplier if it's not a resource
                    if (!resourceData.AllResourceSet.Contains(y.FactorType))
                        return _baseYield[y.FactorType];

                    if (!yieldModifierMultiplier.TryGetValue(y.FactorType, out var yieldMultiplier))
                        yieldMultiplier = 0;

                    if (yieldModifierMultiplier.TryGetValue("AnyResource", out var anyResource))
                        yieldMultiplier += anyResource;

                    return _baseYield[y.FactorType] * (1 + yieldMultiplier / 100f) * (1 + Pop.YieldMultiplier / 100f) *
                           (1 + yieldAdjacencyMultiplier / 100f);
                }

                _yield.Add(new GameFactorChange(YieldGetter, y.FactorType));
            }

            foreach (var u in prototype.Upkeep)
            {
                _baseUpkeep.Add(u.FactorType, u.Amount);

                float UpkeepGetter()
                {
                    if (CurrentState != PopSlotState.Occupied) return 0;

                    return _baseUpkeep[u.FactorType];
                }

                _upkeep.Add(new GameFactorChange(UpkeepGetter, u.FactorType));
            }
        }

        private void OnPopSlotAssignedSignal(ISignal s)
        {
            if (!(s is PopSlotAssignedSignal psas)) return;

            if (this != psas.AssignedSlot) return;
            Pop = psas.Pop;

            CurrentState = PopSlotState.Occupied;
        }

        private void OnPopTrainingStatusChangeSignal(ISignal s)
        {
            if (!(s is PopTrainingStatusChangeSignal ptscs)) return;
            if (ptscs.DestinationSlot != this) return;

            CurrentState = ptscs.IsQuiting ? PopSlotState.Empty : PopSlotState.TrainingForHere;
        }
    }
}
