using Assets._Game.Scripts.Entities.Units;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class AppearanceModule : EntityModuleBase
    {
        private readonly UnitsController _units;
        private readonly UnitFactory _entityUnitFactory;

        public AppearanceModule(UnitsController units, UnitFactory entityUnitFactory)
        {
            _units = units;
            _entityUnitFactory = entityUnitFactory;
        }

        public Unit EnsureUnit(string path, int relativeOrderInLayer)
        {
            var unit = _units.GetUnit(path);
            if (unit != null)
            {
                unit.RelativeOrderInLayer = relativeOrderInLayer;
                return unit;
            }

            unit = _entityUnitFactory.Create(path, relativeOrderInLayer);
            _units.AddUnit(unit);
            return unit;
        }

        public void SetUnitSprite(string path, Sprite sprite)
        {
            var unit = _units.GetUnit(path);
            if (unit != null)
            {
                unit.SpriteRenderer.sprite = sprite;
            }
        }

        public void RemoveUnit(string path)
        {
            _units.RemoveUnit(path);
        }

        public void SetAnimation(EntityAnimationClipName clipName, AnimationClip clip)
        {
            _units.AnimatorController.SetAnimation(clipName, clip);
        }

        public void UpdateOrderInLayer()
        {
            _units.UpdateOrderInLayer();
        }

        public void SetDirection(bool right)
        {
            _units.SetDirection(right);
        }
    }
}
