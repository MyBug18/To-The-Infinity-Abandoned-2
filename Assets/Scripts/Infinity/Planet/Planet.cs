using System.Collections.Generic;
using Infinity.HexTileMap;
using Infinity.Modifiers;

namespace Infinity.Planet
{
    public class Planet : OnHexTileObject, IModifierAttachable, IAffectedByNextTurn
    {
        public readonly bool IsInhabitable;

        public readonly int Size;

        public readonly TileMap TileMap;

        public readonly List<Pop> Pops = new List<Pop>();

        public readonly List<Pop> UnemployedPops = new List<Pop>();

        public readonly EventHandler EventHandler;

        private readonly List<BasicModifier> modifiers = new List<BasicModifier>();

        public Planet(HexTileCoord coord, string name, int size, bool isInhabitable = false) : base(coord, name)
        {
            EventHandler = new EventHandler();
            EventHandler.Subscribe<TileClickEvent>(OnTileClickEvent);

            IsInhabitable = isInhabitable;
            Size = size;

            // for test
            TileMap = new TileMap(4, EventHandler);
        }

        public override void ApplyModifier(BasicModifier modifier)
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

        public void OnNextTurn()
        {

        }

        private void OnTileClickEvent(Event e)
        {
            var tce = e as TileClickEvent;
            if (tce == null) return;

            var coord = tce.Coord;
            UnityEngine.Debug.Log(TileMap[coord].Coord + " Clicked! ( " + coord + " )");
        }
    }
}
