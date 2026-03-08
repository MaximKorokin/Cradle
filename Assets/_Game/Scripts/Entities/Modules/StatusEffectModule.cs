using Assets._Game.Scripts.Entities.StatusEffects;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class StatusEffectModule : EntityModuleBase
    {
        private readonly StatusEffectsController _statusEffectsController;

        public StatusEffectsController StatusEffects => _statusEffectsController;

        public StatusEffectModule(StatusEffectsController statusEffectsController)
        {
            _statusEffectsController = statusEffectsController;

            _statusEffectsController.StatusEffectChanged += OnStatusEffectChanged;
        }

        private void OnStatusEffectChanged(StatusEffectChange change)
        {
            Publish(new StatusEffectChangedEvent(Entity, change));
        }
    }

    public readonly struct StatusEffectChangedEvent : IEntityEvent
    {
        public readonly StatusEffect StatusEffect;
        public readonly StatusEffectChangeKind Kind;

        public Entity Entity { get; }

        public StatusEffectChangedEvent(Entity entity, StatusEffectChange statusEffectChange)
        {
            Entity = entity;
            StatusEffect = statusEffectChange.StatusEffect;
            Kind = statusEffectChange.Kind;
        }
    }
}
