﻿using System.Collections.Generic;
using Infinity.HexTileMap;
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

        public static List<T> GetRandomElements<T>(this IReadOnlyCollection<T> list, int n)
        {
            var result = new List<T>();

            var left = n;
            var count = list.Count;

            foreach (var t in list)
            {
                var chance = left / (float) count;
                if (Random.Range(0f, 1f) < chance)
                {
                    result.Add(t);
                    left--;
                }

                count--;

                if (result.Count == n) break;
            }

            return result;
        }

        public static int ClampInt(int min, int max, int n) => Mathf.Min(Mathf.Max(n, min), max);

        public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict, TKey key,
            TValue value = default)
        {
            return dict.ContainsKey(key) ? dict[key] : value;
        }

        public static int GetDistance(this HexTileCoord c1, HexTileCoord c2) =>
            (Mathf.Abs(c1.Q - c2.Q) + Mathf.Abs(c1.Q + c1.R - c2.Q - c2.R) + Mathf.Abs(c1.R - c2.R)) / 2;
    }
}