using UnityEngine;

namespace Assets._Game.Scripts.Entities.Units
{
    public class UnitsController
    {
        private readonly UnitTree _tree;

        private readonly Transform _unitsRoot;
        private readonly UnitFactory _unitFactory;

        public UnitsController(Transform unitsRoot, Animator animator, UnitFactory unitFactory, AnimatorOverrideController animatorController)
        {
            _unitsRoot = unitsRoot;
            _tree = new(_unitsRoot);
            _unitFactory = unitFactory;
            AnimatorController = new(animator, animatorController);
        }

        public UnitsAnimator AnimatorController { get; private set; }

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

        public void AddUnit(Unit entityUnit)
        {
            _tree.Add(entityUnit);

            AnimatorController.Rebind();
        }

        public Unit GetUnit(string path)
        {
            return _tree.TryGet(path, out var unit) ? unit : null;
        }

        public void RemoveUnit(string path)
        {
            _tree.RemoveRecursive(path);
        }

        public void UpdateOrderInLayer()
        {
            var pivotOrderInLayer = -(int)(_unitsRoot.position.y * 100);
            _tree.UpdateOrderInLayer(pivotOrderInLayer);
        }

        public void SetDirection(bool toRight)
        {
            _unitsRoot.localScale = new(toRight ? 1 : -1, 1, 1);
        }
    }
}
