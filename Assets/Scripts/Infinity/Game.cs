using System;
using System.Collections.Generic;
using Infinity.GalaxySystem;

namespace Infinity
{
    public enum GameSpeed
    {
        Slow = 1,
        Normal = 2,
        Fast = 3,
    }

    public class Game : IEventSenderHolder
    {
        /// <summary>
        /// How many month pass when a turn goes on
        /// </summary>
        public static int GameSpeed { get; private set; } = 2;

        /// <summary>
        /// How many Months has passed
        /// </summary>
        public int MonthsPassed { get; private set; } = 0;

        public UIEventSender UIEventSender { get; }

        Type IEventSenderHolder.HolderType => typeof(Game);

        public readonly Galaxy Galaxy;

        public Game()
        {
            UIEventSender = new UIEventSender(this);
            Galaxy = new Galaxy(UIEventSender);
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
    }
}
