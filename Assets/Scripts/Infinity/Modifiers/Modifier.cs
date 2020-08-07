using System.Collections.Generic;

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

    public struct Modifier
    {
        public readonly ModifierInfo ModifierInfo;

        /// <summary>
        /// -1 if permanent
        /// </summary>
        public readonly int LeftTurn;

        public Modifier(ModifierInfo info, int turn)
        {
            ModifierInfo = info;
            LeftTurn = turn;
        }

        public Modifier ReduceLeftTurn() => new Modifier(ModifierInfo, LeftTurn - 1);
    }

    public class ModifierInfo
    {
        public readonly string ModifierKey;

        public readonly string Description;

        public readonly ModifierHolderKind TopHolder;

        public readonly IReadOnlyDictionary<string, int> GameFactorMultiplier;

        public override bool Equals(object obj)
        {
            return obj is ModifierInfo b && ModifierKey.Equals(b.ModifierKey);
        }

        public override int GetHashCode()
        {
            return ModifierKey.GetHashCode();
        }
    }
}
