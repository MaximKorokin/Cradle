using Assets._Game.Scripts.Entities.StatusEffects;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class StatusEffectModule : EntityModuleBase
    {
        private readonly StatusEffectsController _statusEffectsController;
        private readonly StatusEffectsTickController _statusEffectsTickController;

        public StatusEffectsController StatusEffectsController => _statusEffectsController;

        public StatusEffectModule(StatusEffectsController statusEffectsController, StatusEffectsTickController statusEffectsTickController)
        {
            _statusEffectsController = statusEffectsController;
            _statusEffectsTickController = statusEffectsTickController;

            _statusEffectsController.StatusEffectChanged += OnStatusEffectChanged;
        }

        private void OnStatusEffectChanged(StatusEffectChange change)
        {
            Publish(new StatusEffectChangedEvent(change));
        }

        public override void Dispose()
        {
            base.Dispose();

            _statusEffectsTickController.Dispose();
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
