using System.Collections.Generic;
using Infinity.GameData;
using Infinity.HexTileMap;

namespace Infinity.PlanetPop.BuildingCore
{
    public class PlanetBuildingFactory
    {

        private readonly Neuron _planetNeuron;

        private readonly Planet _planet;

        private BuildingData _buildingData;

        private readonly List<(BuildingQueueElement Element, int LeftTurn)> _constructionQueue =
            new List<(BuildingQueueElement Element, int LeftTurn)>();

        public IReadOnlyList<(BuildingQueueElement Element, int LeftTurn)> ConstructionQueue => _constructionQueue;

        public PlanetBuildingFactory(Neuron planetNeuron, Planet planet)
        {
            _planetNeuron = planetNeuron;
            _planet = planet;

            _buildingData = GameDataStorage.Instance.GetGameData<BuildingData>();
        }

        public void StartConstruction(string buildingName, HexTileCoord coord)
        {
            var prototype = _buildingData[buildingName];
            var newElement = new BuildingQueueElement(prototype, coord);
            _constructionQueue.Add((newElement, prototype.BaseConstructTime));
        }

        private void ProceedConstruction()
        {
            _constructionQueue[0] = (_constructionQueue[0].Element, _constructionQueue[0].LeftTurn - 1);

            if (_constructionQueue[0].LeftTurn == 0)
                EndConstruction();
        }

        private void EndConstruction()
        {
            var completedElement = _constructionQueue[0];
            _constructionQueue.RemoveAt(0);
        }
    }

    public class BuildingQueueElement
    {
        public readonly BuildingPrototype Prototype;

        public readonly HexTileCoord Coord;

        public readonly bool IsUpgrading;

        public BuildingQueueElement(BuildingPrototype prototype, HexTileCoord coord, bool isUpgrading = false)
        {
            Prototype = prototype;
            Coord = coord;
            IsUpgrading = isUpgrading;
        }
    }
}
