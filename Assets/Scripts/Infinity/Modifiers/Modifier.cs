using System.Collections.Generic;
using Infinity.HexTileMap;

namespace Infinity.Modifiers
{
    public enum ModifierHolderKind
    {
        None,
        Game,
        Planet,
        IndividualTile,
        IndividualPop,
    }

    public readonly struct Modifier
    {
        public readonly ModifierInfo ModifierInfo;

        /// <summary>
        /// -1 if permanent
        /// </summary>
        public readonly int LeftTurn;

        /// <summary>
        /// Null if not for tiles
        /// </summary>
        public readonly IReadOnlyCollection<HexTileCoord> AffectedTiles;

        public Modifier(ModifierInfo info, int turn, IReadOnlyCollection<HexTileCoord> affectedTiles = null)
        {
            ModifierInfo = info;
            LeftTurn = turn;
            AffectedTiles = affectedTiles;
        }

        public Modifier ReduceLeftTurn() =>
            new Modifier(ModifierInfo, LeftTurn != -1 ? LeftTurn - 1 : LeftTurn, AffectedTiles);
    }

    public class ModifierInfo
    {
        public readonly string ModifierKey;

        public readonly string ModifierGroup;

        public readonly string Description;

        public readonly ModifierHolderKind TopHolder;

        public readonly IReadOnlyDictionary<string, int> GameFactorAmount;

        public override bool Equals(object obj)
        {
            return obj is ModifierInfo b && ModifierKey.Equals(b.ModifierKey);
        }

        public override int GetHashCode()
        {
            return ModifierKey.GetHashCode();
        }
    }

    public class ModifierSignal : ISignal
    {
        public Neuron FromNeuron { get; }

        public readonly Modifier Modifier;

        public readonly bool IsAdding;

        public bool IsForTile => Modifier.AffectedTiles != null;

        public ModifierSignal(Neuron neuron, Modifier modifier, bool isAdding)
        {
            FromNeuron = neuron;
            Modifier = modifier;
            IsAdding = isAdding;
        }
    }
}
