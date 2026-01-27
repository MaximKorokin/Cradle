using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets._Game.Scripts.Entities
{
    public class EntityUnitsController
    {
        private readonly Dictionary<string, EntityUnit> _units = new();

        private readonly GameObject _unitsRoot;

        public EntityUnitsController(Animator animator)
        {
            AnimatorController = new(animator);
            _unitsRoot = animator.gameObject;
        }

        public EntityUnitsAnimator AnimatorController { get; private set; }

        public void AddUnit(EntityUnit entityUnit)
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

        public EntityUnit GetUnit(string path)
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
