using System.Collections.Generic;
using Infinity.HexTileMap;
using Infinity.Modifiers;

namespace Infinity.Planet
{
    public class Planet : IOnHexTileObject, IModifierAttachable, IAffectedByNextTurn
    {
        public string Name { get; private set; }

        public HexTileCoord HexCoord { get; private set; }

        public readonly bool IsInhabitable;

        public readonly int Size;

        public readonly TileMap TileMap;

        public readonly List<Pop> Pops = new List<Pop>();

        public readonly List<Pop> UnemployedPops = new List<Pop>();

        public readonly EventHandler EventHandler;

        private readonly List<BasicModifier> modifiers = new List<BasicModifier>();

        private ResourceTank resourceTank;

        public Planet(string name, HexTileCoord coord, int size, bool isInhabitable = false)
        {
            HexCoord = coord;
            Name = name;

            EventHandler = new EventHandler();
            EventHandler.Subscribe<TileClickEvent>(OnTileClickEvent);

            resourceTank = new ResourceTank(EventHandler);

            IsInhabitable = isInhabitable;
            Size = size;

            // for test
            TileMap = new TileMap(4, EventHandler);
        }

        public void OnNextTurn()
        {

        }

        public void ApplyModifier(BasicModifier modifier)
        {

        }

        public void AddModifier(BasicModifier modifier)
        {
            modifiers.Add(modifier);
        }

        public void RemoveModifier(BasicModifier modifier)
        {
            modifiers.Remove(modifier);
        }

        private void OnTileClickEvent(Event e)
        {
            if (!(e is TileClickEvent tce)) return;

            var coord = tce.Coord;
            UnityEngine.Debug.Log(TileMap[coord].Coord + " Clicked! ( " + coord + " )");
        }
    }
}
