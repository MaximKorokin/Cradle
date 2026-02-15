namespace Assets._Game.Scripts.Entities.StatusEffects
{
    public sealed class StatusEffectAssembler
    {
        private readonly StatusEffectSystem _statusEffectSystem;

        public StatusEffectAssembler(StatusEffectSystem statusEffectSystem)
        {
            _statusEffectSystem = statusEffectSystem;
        }

        public StatusEffect Assemble(StatusEffectDefinition definition, StatusEffectsController controller)
        {
            var statusEffect = new StatusEffect(definition);
            if (controller.AddStatusEffect(statusEffect))
            {
                _statusEffectSystem.AddStatusEffect(statusEffect);
                statusEffect.Expired += () => OnStatusEffectExpired(statusEffect, controller);
            }
            return statusEffect;
        }

        private void OnStatusEffectExpired(StatusEffect statusEffect, StatusEffectsController controller)
        {
            controller.RemoveStatusEffect(statusEffect);
            _statusEffectSystem.RemoveStatusEffect(statusEffect);
        }
    }
}
