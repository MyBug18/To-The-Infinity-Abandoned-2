using System.Collections.Generic;
using Infinity.HexTileMap;
using Infinity.Modifiers;

namespace Infinity.PlanetPop
{
    public class Pop : IModifierHolder
    {
        private readonly Dictionary<string, BasicModifier> _modifiers = new Dictionary<string, BasicModifier>();

        public IReadOnlyDictionary<string, BasicModifier> Modifiers => _modifiers;

        public string Name { get; private set; }

        public HexTileCoord CurrentCoord { get; private set; }

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