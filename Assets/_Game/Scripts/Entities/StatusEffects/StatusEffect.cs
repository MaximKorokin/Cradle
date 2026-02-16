using Assets.CoreScripts;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.StatusEffects
{
    public sealed class StatusEffect
    {
        public StatusEffectDefinition Definition { get; }
        public float RemainingDuration => _cooldownCounter.Cooldown - _cooldownCounter.TimeSinceReset;
        public StatusEffectSnapshot Snapshot => new(this);

        private readonly CooldownCounter _cooldownCounter;

        public event Action Expired;
        public event Action Ticked;

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
            Ticked?.Invoke();
        }
    }

    public readonly struct StatusEffectSnapshot
    {
        public readonly string Name;
        public readonly Sprite Icon;
        public readonly float Duration;
        public readonly float RemainingDuration;

        public StatusEffectSnapshot(StatusEffect statusEffect)
        {
            Name = statusEffect.Definition.Name;
            Icon = statusEffect.Definition.Icon;
            Duration = statusEffect.Definition.Duration;
            RemainingDuration = statusEffect.RemainingDuration;
        }
    }
}
