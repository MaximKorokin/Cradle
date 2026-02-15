using Assets._Game.Scripts.Entities.StatusEffects;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class StatusEffectModule : EntityModuleBase
    {
        private readonly StatusEffectsController _statusEffectsController;

        public StatusEffectsController StatusEffectsController => _statusEffectsController;

        public StatusEffectModule(StatusEffectsConfig config)
        {
            _statusEffectsController = new(config);
            _statusEffectsController.StatusEffectChanged += OnStatusEffectChanged;
        }

        private void OnStatusEffectChanged(StatusEffectChange change)
        {
            Publish(new StatusEffectChangedEvent(change));
        }
    }

    public readonly struct StatusEffectChangedEvent : IEntityEvent
    {
        public readonly StatusEffect StatusEffect;
        public readonly StatusEffectChangeKind Kind;

        public StatusEffectChangedEvent(StatusEffectChange statusEffectChange)
        {
            StatusEffect = statusEffectChange.StatusEffect;
            Kind = statusEffectChange.Kind;
        }
    }
}
