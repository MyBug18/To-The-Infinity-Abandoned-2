﻿using System;
using System.Collections.Generic;
using System.Linq;
using Infinity.GameData;
using Infinity.HexTileMap;

namespace Infinity.PlanetPop.BuildingCore
{
    public class PlanetBuildingFactory
    {
        private readonly Neuron _planetNeuron;

        private readonly Planet _planet;

        private readonly BuildingData _buildingData;

        private readonly List<(BuildingQueueElement Element, int LeftTurn)> _constructionQueue =
            new List<(BuildingQueueElement Element, int LeftTurn)>();

        public IReadOnlyList<(BuildingQueueElement Element, int LeftTurn)> ConstructionQueue => _constructionQueue;

        public PlanetBuildingFactory(Neuron planetNeuron, Planet planet)
        {
            _planetNeuron = planetNeuron;
            _planet = planet;

            _buildingData = GameDataStorage.Instance.GetGameData<BuildingData>();

            _planetNeuron.Subscribe<GameCommandSignal>(ProceedConstruction);
        }

        public void StartConstruction(string buildingName, HexTileCoord coord)
        {
            if (_constructionQueue.Any(q => q.Element.Coord == coord))
                throw new InvalidOperationException();

            //TODO: Add resource consumption
            var prototype = _buildingData[buildingName];

            if (!prototype.CheckWholeCondition(_planet, coord))
                throw new InvalidOperationException();

            var newElement = new BuildingQueueElement(prototype, coord);
            _constructionQueue.Add((newElement, prototype.BaseConstructTime));

            _planetNeuron.SendSignal(
                new BuildingQueueChangeSignal(_planetNeuron, newElement, BuildingQueueChangeType.Added),
                SignalDirection.Local);
        }

        public void CancelConstruction(int index)
        {
            if (index < 0 || index > _constructionQueue.Count - 1)
                throw new IndexOutOfRangeException();

            var (element, _) = _constructionQueue[index];
            _constructionQueue.RemoveAt(index);

            _planetNeuron.SendSignal(
                new BuildingQueueChangeSignal(_planetNeuron, element, BuildingQueueChangeType.Canceled),
                SignalDirection.Local);
        }

        public void CancelConstruction(HexTileCoord coord)
        {
            if (_constructionQueue.All(q => q.Element.Coord != coord))
                throw new InvalidOperationException();

            BuildingQueueElement element = null;

            var walker = 0;
            for (; walker < _constructionQueue.Count; walker++)
            {
                var (elem, _) = _constructionQueue[walker];
                if (elem.Coord != coord) continue;

                element = elem;
                break;
            }

            if (element == null)
                throw new InvalidOperationException();

            _constructionQueue.RemoveAt(walker);

            _planetNeuron.SendSignal(
                new BuildingQueueChangeSignal(_planetNeuron, element, BuildingQueueChangeType.Canceled),
                SignalDirection.Local);
        }

        private void ProceedConstruction(ISignal s)
        {
            if (!(s is GameCommandSignal gcs) || gcs.CommandType != GameCommandType.StartNewTurn) return;

            _constructionQueue[0] = (_constructionQueue[0].Element, _constructionQueue[0].LeftTurn - 1);

            if (_constructionQueue[0].LeftTurn == 0)
                EndConstruction();
        }

        private void EndConstruction()
        {
            var (element, _) = _constructionQueue[0];
            _constructionQueue.RemoveAt(0);
            _planetNeuron.SendSignal(
                new BuildingQueueChangeSignal(_planetNeuron, element, BuildingQueueChangeType.Ended),
                SignalDirection.Local);
        }

        public void MoveToSmallerPosition(int index)
        {
            if (index <= 0)
                throw new ArgumentOutOfRangeException();

            var temp = _constructionQueue[index - 1];
            _constructionQueue[index - 1] = _constructionQueue[index];
            _constructionQueue[index] = temp;
        }

        public void MoveToBiggerPosition(int index)
        {
            if (index >= _constructionQueue.Count - 1)
                throw new ArgumentOutOfRangeException();

            var temp = _constructionQueue[index + 1];
            _constructionQueue[index + 1] = _constructionQueue[index];
            _constructionQueue[index] = temp;
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

    public class BuildingQueueChangeSignal : ISignal
    {
        public Neuron FromNeuron { get; }

        public readonly BuildingQueueElement QueueElement;

        public readonly BuildingQueueChangeType Type;

        public BuildingQueueChangeSignal(Neuron neuron, BuildingQueueElement building, BuildingQueueChangeType type)
        {
            FromNeuron = neuron;
            QueueElement = building;
            Type = type;
        }
    }

    public enum BuildingQueueChangeType
    {
        Ended,
        Canceled,
        Added,
    }
}
