using Assets._Game.Scripts.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Game.Scripts.Entities
{
    public class EntityUnitsController
    {
        private readonly Dictionary<string, EntityUnit> _units = new();

        private readonly GameObject _unitsRoot;
        private readonly Animator _animator;

        public EntityUnitsController(Animator animator)
        {
            _animator = animator;
            _unitsRoot = _animator.gameObject;
        }

        public void AddUnit(EntityUnit entityUnit)
        {
            _unitsRoot.transform.AddChild(entityUnit.GameObject.transform, entityUnit.Path);
            _units.Add(entityUnit.Path, entityUnit);
            _animator.Rebind();
            _animator.Update(0f);
        }

        public EntityUnit GetUnit(string path)
        {
            return _units[path];
        }

        public void PlayAnimationByTrigger(string trigger)
        {
            _animator.SetTrigger(trigger);
        }

        public void SetDirection(bool right)
        {
            _unitsRoot.transform.localScale = new(right ? 1 : -1, 1, 1);
        }
    }
}
