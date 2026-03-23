using System;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Units
{
    public class UnitsController
    {
        private readonly UnitTree _tree;

        private readonly Transform _unitsRoot;
        private readonly UnitViewProvider _unitViewProvider;
        private readonly bool _swapOrderInLayerForDirection;

        public event Action Changed;

        public UnitsController(Transform unitsRoot, UnitViewProvider unitViewProvider, bool swapOrderInLayerForDirection)
        {
            _unitsRoot = unitsRoot;
            _tree = new(_unitsRoot, TurnDirection.Right);
            _unitViewProvider = unitViewProvider;
            _swapOrderInLayerForDirection = swapOrderInLayerForDirection;
        }

        public void EnsureUnit(string path, int relativeOrderInLayer)
        {
            var unit = GetUnit(path);
            if (unit != null)
            {
                unit.RelativeOrderInLayer = relativeOrderInLayer;
                return;
            }

            unit = _unitViewProvider.Create(path, relativeOrderInLayer);
            AddUnit(unit);
        }

        public void SetUnitSprite(string path, Sprite sprite)
        {
            var unit = GetUnit(path);
            if (unit != null)
            {
                unit.SpriteRenderer.sprite = sprite;
            }
        }

        public void AddUnit(UnitView entityUnit)
        {
            _tree.Add(entityUnit);

            Changed?.Invoke();
        }

        public UnitView GetUnit(string path)
        {
            return _tree.TryGet(path, out var unit) ? unit : null;
        }

        public void RemoveUnit(string path)
        {
            var unit = GetUnit(path);
            var removed = _tree.RemoveRecursive(path, false);
            if (removed != null && removed.Count > 0)
            {
                for (int i = 0; i < removed.Count; i++)
                    _unitViewProvider.Destroy(removed[i]);
                Changed?.Invoke();
            }
        }

        public void UpdateOrderInLayer()
        {
            var pivotOrderInLayer = -(int)(_unitsRoot.position.y * 100);
            _tree.UpdateOrderInLayer(pivotOrderInLayer);
        }

        public void SetTurnDirection(TurnDirection turnDirection)
        {
            _unitsRoot.localScale = new(turnDirection == TurnDirection.Right ? 1 : -1, 1, 1);
            
            if (_swapOrderInLayerForDirection)
                _tree.SetTurnDirection(turnDirection);
        }

        public void ClearUnits()
        {
            _tree.ExecuteAllDepthFirst(u => RemoveUnit(u.Path));
        }
    }
}
