using Assets.CoreScripts;
using System;

namespace Assets._Game.Scripts.Entities.StatusEffects
{
    public sealed class StatusEffect
    {
        public StatusEffectDefinition Definition { get; }
        public float RemainingDuration => _cooldownCounter.Cooldown - _cooldownCounter.TimeSinceReset;

        private readonly CooldownCounter _cooldownCounter;

        public event Action Expired;

        public StatusEffect(StatusEffectDefinition definition)
        {
            Definition = definition;
            _cooldownCounter = new(definition.Duration);
            _cooldownCounter.Reset();
        }

        public void OnTick()
        {
            if (_cooldownCounter.IsOver())
            {
                Expired?.Invoke();
            }
        }
    }
}
