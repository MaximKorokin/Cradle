using UnityEngine;

namespace Assets._Game.Scripts.Entities.Units
{
    public class UnitsController
    {
        private readonly UnitTree _tree;

        private readonly Transform _unitsRoot;

        public UnitsController(Transform unitsRoot, Animator animator, AnimatorOverrideController animatorController)
        {
            _unitsRoot = unitsRoot;
            _tree = new(_unitsRoot);
            AnimatorController = new(animator, animatorController);
        }

        public UnitsAnimator AnimatorController { get; private set; }

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

        public void SetDirection(bool right)
        {
            _unitsRoot.localScale = new(right ? 1 : -1, 1, 1);
        }
    }
}
