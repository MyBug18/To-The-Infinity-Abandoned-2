using Infinity.HexTileMap;
using Infinity.Modifiers;
using System.Collections.Generic;

namespace Infinity.PlanetPop
{
    public class Pop : IModifierHolder
    {
        private readonly Dictionary<string, BasicModifier> _modifiers = new Dictionary<string, BasicModifier>();

        public IReadOnlyDictionary<string, BasicModifier> Modifiers => _modifiers;

        public string Name { get; private set; }

        public HexTileCoord CurrentCoord { get; private set; }

        public int YieldMultiplier => 0;

        public int UpkeepMultiplier => 0;

        public Pop(string name, HexTileCoord initialCoord)
        {
            Name = name;
            CurrentCoord = initialCoord;
        }
    }

    public class PopBirthSignal : ISignal
    {
        public ISignalDispatcherHolder SignalSender { get; }

        public readonly Pop Pop;

        public PopBirthSignal(ISignalDispatcherHolder sender, Pop pop)
        {
            SignalSender = sender;
            Pop = pop;
        }
    }
}
