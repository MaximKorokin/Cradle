using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Units;
using Assets._Game.Scripts.Infrastructure.Storage;
using UnityEngine;

namespace Assets._Game.Scripts.Entities
{
    public sealed class EntityView : MonoBehaviour, IEntry, IPoolable
    {
        string IEntry.Id { get; set; }
        Component IPoolable.Prefab { get; set; }

        [field: SerializeField]
        public Transform UnitsRoot { get; private set; }
        [field: SerializeField]
        public Animator UnitsAnimator { get; private set; }

        public UnitsController UnitsController { get; private set; }
        public UnitsAnimator AnimatorController { get; private set; }

        private AppearanceModule _appearance;

        public void Initialize(UnitsController unitsController)
        {
            UnitsController = unitsController;
            UnitsController.Changed += OnUnitsControllerChanged;
        }

        private void OnUnitsControllerChanged()
        {
            AnimatorController?.Rebind();
        }

        public void Bind(AppearanceModule appearance)
        {
            AnimatorController = new(UnitsAnimator, appearance.EntityVisualModel.Animator);
            AnimatorController.Rebind();

            if (_appearance != null) Unbind();
            _appearance = appearance;

            _appearance.EnsureUnitRequested += UnitsController.EnsureUnit;
            _appearance.SetUnitSpriteRequested += UnitsController.SetUnitSprite;
            _appearance.RemoveUnitRequested += UnitsController.RemoveUnit;
            _appearance.SetDirectionRequested += UnitsController.SetDirection;
            _appearance.UpdateOrderInLayerRequested += UnitsController.UpdateOrderInLayer;

            _appearance.SetAnimationRequested += AnimatorController.SetAnimation;
        }

        public void Unbind()
        {
            if (_appearance == null) return;

            _appearance.EnsureUnitRequested -= UnitsController.EnsureUnit;
            _appearance.SetUnitSpriteRequested -= UnitsController.SetUnitSprite;
            _appearance.RemoveUnitRequested -= UnitsController.RemoveUnit;
            _appearance.SetDirectionRequested -= UnitsController.SetDirection;
            _appearance.UpdateOrderInLayerRequested -= UnitsController.UpdateOrderInLayer;

            _appearance.SetAnimationRequested -= AnimatorController.SetAnimation;

            _appearance = null;
        }

        private void OnDisable()
        {
            if (_appearance != null)
            {
                Unbind();
            }
        }

        public void OnTake()
        {

        }

        public void OnReturn()
        {

        }
    }
}
