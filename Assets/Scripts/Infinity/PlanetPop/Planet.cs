using System;
using System.Collections.Generic;
using System.Linq;
using Infinity.HexTileMap;
using Infinity.Modifiers;

namespace Infinity.PlanetPop
{
    public class Planet : IOnHexTileObject, IModifierAttachable, IAffectedByNextTurn, IEventHandlerHolder
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

        private readonly Dictionary<ResourceType, float> _currentResource = new Dictionary<ResourceType, float>();

        private readonly Dictionary<ResourceType, Dictionary<ResourceChangeType, float>> _detailedTurnResource =
            new Dictionary<ResourceType, Dictionary<ResourceChangeType, float>>();

        private readonly Dictionary<ResourceType, int> _turnResourceMultiplier = new Dictionary<ResourceType, int>();

        /// <summary>
        /// Planetary resources only
        /// </summary>
        public IReadOnlyDictionary<ResourceType, float> CurrentResource => _currentResource;

        public IReadOnlyDictionary<ResourceType, Dictionary<ResourceChangeType, float>> DetailedTurnResource => _detailedTurnResource;

        public IReadOnlyDictionary<ResourceType, int> TurnResourceMultiplier => _turnResourceMultiplier;

        #endregion

        private readonly List<BasicModifier> _modifiers = new List<BasicModifier>();

        public Planet(string name, HexTileCoord coord, int size, bool isInhabitable = false)
        {
            HexCoord = coord;
            Name = name;
            IsInhabitable = isInhabitable;
            Size = size;

            // for test
            EventHandler = new EventHandler(this);
            EventHandler.Subscribe<TileClickEvent>(OnTileClickEvent);

            for (var r = ResourceType.Energy; r <= ResourceType.Alloy; r++)
                _currentResource.Add(r, 0);

            for (var r = ResourceType.Energy; r <= ResourceType.EngineerResearch; r++)
            {
                var changeDict =
                    ((ResourceChangeType[]) Enum.GetValues(typeof(ResourceChangeType)))
                    .ToDictionary<ResourceChangeType, ResourceChangeType, float>(i => i, i => 0);

                _detailedTurnResource.Add(r, changeDict);
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
            var keys = _currentResource.Keys;

            foreach (var i in keys)
            {

            }
        }

        public void ApplyModifier(BasicModifier modifier)
        {

        }

        public void AddModifier(BasicModifier modifier)
        {
            _modifiers.Add(modifier);
        }

        public void RemoveModifier(BasicModifier modifier)
        {
            _modifiers.Remove(modifier);
        }

        private void OnTileClickEvent(Event e, IEventHandlerHolder holder)
        {
            if (!(e is TileClickEvent tce)) return;

            var coord = tce.Coord;
            UnityEngine.Debug.Log(TileMap[coord].Coord + " Clicked! ( " + coord + " )");
        }

        Type IEventHandlerHolder.GetHolderType()
        {
            return typeof(Planet);
        }
    }
}
