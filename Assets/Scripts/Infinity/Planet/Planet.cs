using System;
using System.Collections.Generic;
using System.Linq;
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

        public readonly EventHandler EventHandler;

        #region Pop

        public readonly List<Pop> Pops = new List<Pop>();

        public readonly List<Pop> UnemployedPops = new List<Pop>();

        public const int BasePopGrowth = 3;

        #endregion

        #region Resources

        private readonly Dictionary<ResourceType, float> currentResource = new Dictionary<ResourceType, float>();

        private readonly Dictionary<ResourceType, Dictionary<ResourceChangeType, float>> detailedTurnResource =
            new Dictionary<ResourceType, Dictionary<ResourceChangeType, float>>();

        private readonly Dictionary<ResourceType, int> turnResourceMultiplier = new Dictionary<ResourceType, int>();

        /// <summary>
        /// Planetary resources only
        /// </summary>
        public IReadOnlyDictionary<ResourceType, float> CurrentResource => currentResource;

        public IReadOnlyDictionary<ResourceType, Dictionary<ResourceChangeType, float>> DetailedTurnResource => detailedTurnResource;

        public IReadOnlyDictionary<ResourceType, int> TurnResourceMultiplier => turnResourceMultiplier;

        #endregion

        private readonly List<BasicModifier> modifiers = new List<BasicModifier>();

        public Planet(string name, HexTileCoord coord, int size, bool isInhabitable = false)
        {
            HexCoord = coord;
            Name = name;
            IsInhabitable = isInhabitable;
            Size = size;

            EventHandler = new EventHandler();
            EventHandler.Subscribe<TileClickEvent>(OnTileClickEvent);

            for (var r = ResourceType.Energy; r <= ResourceType.Alloy; r++)
                currentResource.Add(r, 0);

            for (var r = ResourceType.Energy; r <= ResourceType.EngineerResearch; r++)
            {
                var changeDict =
                    ((ResourceChangeType[]) Enum.GetValues(typeof(ResourceChangeType)))
                    .ToDictionary<ResourceChangeType, ResourceChangeType, float>(i => i, i => 0);

                detailedTurnResource.Add(r, changeDict);
            }

            // for test
            TileMap = new TileMap(4, EventHandler);
        }

        public void OnNextTurn()
        {
            ApplyTurnResource();
        }

        /// <summary>
        /// Planetary resources only
        /// </summary>
        private void ApplyTurnResource()
        {
            var keys = currentResource.Keys;

            foreach (var i in keys)
            {

            }
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
