using System;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Units
{
    public class UnitsController
    {
        private readonly UnitTree _tree;

        private readonly Transform _unitsRoot;
        private readonly UnitFactory _unitFactory;

        public event Action Changed;

        public UnitsController(Transform unitsRoot, UnitFactory unitFactory)
        {
            _unitsRoot = unitsRoot;
            _tree = new(_unitsRoot);
            _unitFactory = unitFactory;
        }

        public void EnsureUnit(string path, int relativeOrderInLayer)
        {
            var unit = GetUnit(path);
            if (unit != null)
            {
                unit.RelativeOrderInLayer = relativeOrderInLayer;
                return;
            }

            unit = _unitFactory.Create(path, relativeOrderInLayer);
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
            _tree.RemoveRecursive(path);
            Changed?.Invoke();
        }

        public void UpdateOrderInLayer()
        {
            var pivotOrderInLayer = -(int)(_unitsRoot.position.y * 100);
            _tree.UpdateOrderInLayer(pivotOrderInLayer);
            Changed?.Invoke();
        }

        public void SetDirection(bool toRight)
        {
            _unitsRoot.localScale = new(toRight ? 1 : -1, 1, 1);
        }
    }
}
