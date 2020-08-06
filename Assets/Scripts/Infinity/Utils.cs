using UnityEngine;

namespace Infinity
{
    public static class Utils
    {
        public static bool GetBoolFromChance(int chance)
        {
            var decider = Random.Range(1, 100);
            return decider <= chance;
        }
    }
}