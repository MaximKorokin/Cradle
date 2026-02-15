using Assets._Game.Scripts.Infrastructure;
using Assets.CoreScripts;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Entities.StatusEffects
{
    public sealed class StatusEffectSystem
    {
        private readonly HashSet<StatusEffect> _activeStatusEffects = new();
        private readonly HashSet<StatusEffect> _addPendingStatusEffects = new();
        private readonly HashSet<StatusEffect> _removePendingStatusEffects = new();
        private readonly CooldownCounter _cooldownCounter;

        public StatusEffectSystem(Dispatcher dispatcher, StatusEffectsConfig config)
        {
            if (config.TickRate <= 0) throw new System.ArgumentException("TickRate must be greater than 0", nameof(config.TickRate));
            _cooldownCounter = new(1 / config.TickRate);

            dispatcher.OnTick += OnTick;
        }

        private void OnTick()
        {
            if (_cooldownCounter.TryReset())
            {
                foreach (var statusEffect in _activeStatusEffects)
                {
                    statusEffect.OnTick();
                }
            }

            // Apply pending changes after ticking to avoid modifying the collection while iterating
            foreach (var statusEffect in _addPendingStatusEffects)
            {
                _activeStatusEffects.Add(statusEffect);
            }
            foreach (var statusEffect in _removePendingStatusEffects)
            {
                _activeStatusEffects.Remove(statusEffect);
            }
        }

        public void AddStatusEffect(StatusEffect statusEffect)
        {
            _addPendingStatusEffects.Add(statusEffect);
        }

        public void RemoveStatusEffect(StatusEffect statusEffect)
        {
            _removePendingStatusEffects.Add(statusEffect);
        }
    }
}
