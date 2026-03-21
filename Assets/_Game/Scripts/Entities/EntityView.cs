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

        [SerializeField]
        private MonoBehaviour[] _entityBindables;

        public Entity Entity { get; private set; }

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

        public void Bind(Entity entity)
        {
            // Unbind previous if exists
            Unbind();

            Entity = entity;

            for (int i = 0; i < _entityBindables.Length; i++)
            {
                ((IEntityBindable)_entityBindables[i]).Bind(entity);
            }

            if (!entity.TryGetModule(out _appearance))
            {
                SLog.Error($"Entity {entity.Definition.Id} has no {nameof(AppearanceModule)} attached.");
            }

            if (_appearance.EntityVisualModel.Animator != null)
            {
                AnimatorController = new(UnitsAnimator, _appearance.EntityVisualModel.Animator);
                AnimatorController.Rebind();
            }

            _appearance.EnsureUnitRequested += UnitsController.EnsureUnit;
            _appearance.SetUnitSpriteRequested += UnitsController.SetUnitSprite;
            _appearance.RemoveUnitRequested += UnitsController.RemoveUnit;
            _appearance.SetTurnDirectionRequested += UnitsController.SetTurnDirection;
            _appearance.UpdateOrderInLayerRequested += UnitsController.UpdateOrderInLayer;

            if (AnimatorController != null)
            {
                _appearance.SetAnimationRequested += AnimatorController.SetAnimation;
                _appearance.SetAnimatorValueRequested += AnimatorController.SetValue;
            }

            entity.Publish<EntityBoundEvent>(new(entity));
        }

        public void Unbind()
        {
            for (int i = 0; i < _entityBindables.Length; i++)
            {
                ((IEntityBindable)_entityBindables[i]).Unbind();
            }

            if (_appearance == null) return;

            _appearance.EnsureUnitRequested -= UnitsController.EnsureUnit;
            _appearance.SetUnitSpriteRequested -= UnitsController.SetUnitSprite;
            _appearance.RemoveUnitRequested -= UnitsController.RemoveUnit;
            _appearance.SetTurnDirectionRequested -= UnitsController.SetTurnDirection;
            _appearance.UpdateOrderInLayerRequested -= UnitsController.UpdateOrderInLayer;

            if (AnimatorController != null)
            {
                _appearance.SetAnimationRequested -= AnimatorController.SetAnimation;
                _appearance.SetAnimatorValueRequested -= AnimatorController.SetValue;
            }

            _appearance = null;
            Entity = null;
        }

        private void OnDisable()
        {
            if (Entity != null)
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

    public interface IEntityBindable
    {
        void Bind(Entity entity);
        void Unbind();
    }

    public readonly struct EntityBoundEvent : IEntityEvent
    {
        public Entity Entity { get; }

        public EntityBoundEvent(Entity entity) => Entity = entity;
    }
}
