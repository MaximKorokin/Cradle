using Assets.CoreScripts;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Units
{
    public class EntityUnitsController
    {
        private readonly EntityUnitTree _tree;

        private readonly Transform _unitsRoot;

        public EntityUnitsController(Transform unitsRoot, Animator animator)
        {
            _unitsRoot = unitsRoot;
            _tree = new(_unitsRoot);
            AnimatorController = new(animator);
        }

        public EntityUnitsAnimator AnimatorController { get; private set; }

        public void AddUnit(EntityUnit entityUnit)
        {
            _tree.Add(entityUnit);

            AnimatorController.Rebind();
        }

        public EntityUnit GetUnit(string path)
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
