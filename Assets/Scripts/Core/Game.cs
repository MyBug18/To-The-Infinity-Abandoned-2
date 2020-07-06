using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infinity.Core
{
    public class Game : MonoBehaviour
    {
        /// <summary>
        /// How many month pass when a turn goes on
        /// </summary>
        public int GameSpeed { get; private set; } = 2;

        /// <summary>
        /// How many Months has passed
        /// </summary>
        public int MonthsPassed { get; private set; } = 0;

        /// <summary>
        /// End this turn and starts an enemy turn
        /// </summary>
        public void EndTurn()
        {
        }

        /// <summary>
        /// Start new turn
        /// </summary>
        public void StartNewTurn()
        {
            MonthsPassed += GameSpeed;

        }
    }
}
