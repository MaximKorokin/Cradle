using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Units;
using Assets._Game.Scripts.Infrastructure.Storage;
using UnityEngine;

namespace Assets._Game.Scripts.Entities
{
    public sealed class EntityView : MonoBehaviour, IEntry
    {
        string IEntry.Id { get; set; }

        [field: SerializeField]
        public Transform UnitsRoot { get; private set; }
        [field: SerializeField]
        public Animator UnitsAnimator { get; private set; }

        public UnitsController UnitsController { get; set; }

        private AppearanceModule _appearance;

        public void Bind(AppearanceModule appearance)
        {
            _appearance = appearance;

            _appearance.EnsureUnitRequested += UnitsController.EnsureUnit;
            _appearance.SetUnitSpriteRequested += UnitsController.SetUnitSprite;
            _appearance.RemoveUnitRequested += UnitsController.RemoveUnit;
            
            _appearance.SetAnimationRequested += UnitsController.AnimatorController.SetAnimation;
            _appearance.SetDirectionRequested += UnitsController.SetDirection;
            _appearance.UpdateOrderInLayerRequested += UnitsController.UpdateOrderInLayer;
        }

        public void Unbind()
        {
            _appearance.EnsureUnitRequested += UnitsController.EnsureUnit;
            _appearance.SetUnitSpriteRequested += UnitsController.SetUnitSprite;
            _appearance.RemoveUnitRequested += UnitsController.RemoveUnit;

            _appearance.SetAnimationRequested += UnitsController.AnimatorController.SetAnimation;
            _appearance.SetDirectionRequested += UnitsController.SetDirection;
            _appearance.UpdateOrderInLayerRequested += UnitsController.UpdateOrderInLayer;

            _appearance = null;
        }

        private void OnDestroy()
        {
            if (_appearance != null)
            {
                Unbind();
            }
        }
    }
}
