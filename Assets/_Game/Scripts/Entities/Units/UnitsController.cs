using Assets._Game.Scripts.Entities.Modules;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Units
{
    /// <summary>
    /// In other words - EntityVisual
    /// </summary>
    public class UnitsController : IEntityModule
    {
        private readonly Dictionary<string, Unit> _units = new();

        private readonly GameObject _unitsRoot;

        public UnitsController(Animator animator)
        {
            AnimatorController = new(animator);
            _unitsRoot = animator.gameObject;
        }

        public UnitsAnimator AnimatorController { get; private set; }

        public void AddUnit(Unit entityUnit)
        {
            Transform parentTransform;
            if (_units.TryGetValue(Path.GetDirectoryName(entityUnit.Path), out var parentUnit))
            {
                parentUnit.AddChild(entityUnit);
                parentTransform = parentUnit.GameObject.transform;
            }
            else
            {
                parentTransform = _unitsRoot.transform;
            }
            entityUnit.GameObject.transform.parent = parentTransform;

            entityUnit.GameObject.transform.localPosition = Vector3.zero;
            _units.Add(entityUnit.Path, entityUnit);

            AnimatorController.Rebind();
        }

        public Unit GetUnit(string path)
        {
            return _units[path];
        }

        public void UpdateOrderInLayer()
        {
            var pivotOrderInLayer = -(int)(_unitsRoot.transform.position.y * 100);
            foreach (var unit in _units.Values)
            {
                unit.UpdateOrderInLayer(pivotOrderInLayer);
            }
        }

        public void SetDirection(bool right)
        {
            _unitsRoot.transform.localScale = new(right ? 1 : -1, 1, 1);
        }
    }
}
