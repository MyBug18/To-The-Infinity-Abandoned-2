using System;
using System.Collections.Generic;

namespace Infinity
{
    public enum GameSpeed
    {
        Slow = 1,
        Normal = 2,
        Fast = 3,
    }

    public class Game : IEventHandlerHolder
    {
        /// <summary>
        /// How many month pass when a turn goes on
        /// </summary>
        public static int GameSpeed { get; private set; } = 2;

        /// <summary>
        /// How many Months has passed
        /// </summary>
        public int MonthsPassed { get; private set; } = 0;

        public readonly EventHandler TopEventHandler;

        public Game()
        {
            TopEventHandler = new EventHandler(this);
        }

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

        Type IEventHandlerHolder.GetHolderType() => typeof(Game);
    }
}
