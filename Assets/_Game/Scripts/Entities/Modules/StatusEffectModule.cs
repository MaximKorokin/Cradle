using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.Infrastructure.Configs;

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

    public sealed class StatusEffectModuleFactory : IEntityModuleFactory
    {
        private readonly StatusEffectsConfig _config;

        public StatusEffectModuleFactory(StatusEffectsConfig config)
        {
            _config = config;
        }

        public EntityModuleBase Create(EntityDefinition entityDefinition)
        {
            var controller = new StatusEffectsController(_config);
            var module = new StatusEffectModule(controller);
            return module;
        }
    }
}
