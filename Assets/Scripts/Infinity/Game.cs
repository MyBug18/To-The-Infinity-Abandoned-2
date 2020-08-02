using Infinity.GalaxySystem;
using System;

namespace Infinity
{
    public enum GameSpeed
    {
        Slow = 1,
        Normal = 2,
        Fast = 3,
    }

    public class Game : ISignalDispatcherHolder
    {
        private static Game _instance;

        public static Game Instance
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException("Game instance has not yet initialized!");

                return _instance;
            }
        }

        /// <summary>
        /// How many month pass when a turn goes on
        /// </summary>
        public int GameSpeed { get; private set; } = 2;

        /// <summary>
        /// How many Months has passed
        /// </summary>
        public int MonthsPassed { get; private set; }

        private readonly Neuron _neuron;

        Type ISignalDispatcherHolder.HolderType => typeof(Game);

        public SignalDispatcher SignalDispatcher { get; }

        public readonly Galaxy Galaxy;

        public Game()
        {
            if (_instance != null)
                throw new InvalidOperationException("Trying to instantiate new game instance while an instance already exist!");

            _instance = this;

            _neuron = Neuron.GetNeuronForGame();
            SignalDispatcher = new SignalDispatcher(_neuron);

            _neuron.Subscribe<GameEventSignal<Game>>(OnGameEventSignal);

            Galaxy = new Galaxy(_neuron);
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
            _neuron.SendSignal(new NextTurnSignal(this), SignalDirection.Downward);
            MonthsPassed += GameSpeed;
        }

        private void OnGameEventSignal(ISignal s)
        {
            if (!(s is GameEventSignal<Game> ges)) return;
        }
    }

    public class NextTurnSignal : ISignal
    {
        public ISignalDispatcherHolder SignalSender { get; }

        public NextTurnSignal(ISignalDispatcherHolder holder)
        {
            SignalSender = holder;
        }
    }
}
