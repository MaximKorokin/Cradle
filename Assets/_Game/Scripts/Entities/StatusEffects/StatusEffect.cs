using Assets.CoreScripts;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.StatusEffects
{
    public sealed class StatusEffect
    {
        public StatusEffectDefinition Definition { get; }
        public float RemainingDuration => Definition.IsEndless ? Definition.Duration : _cooldownCounter.Cooldown - _cooldownCounter.TimeSinceReset;
        public StatusEffectSnapshot Snapshot => new(this);

        private readonly CooldownCounter _cooldownCounter;

        public event Action<StatusEffect> Expired;
        public event Action<StatusEffect> Ticked;

        public StatusEffect(StatusEffectDefinition definition)
        {
            Definition = definition;
            _cooldownCounter = new(definition.Duration);
            _cooldownCounter.Reset();
        }

        public void OnTick()
        {
            if (!Definition.IsEndless && _cooldownCounter.IsOver())
            {
                Expired?.Invoke(this);
            }
            Ticked?.Invoke(this);
        }
    }

    public readonly struct StatusEffectSnapshot
    {
        public readonly string Name;
        public readonly Sprite Icon;
        public readonly float Duration;
        public readonly float RemainingDuration;
        public readonly bool IsEndless;

        public StatusEffectSnapshot(StatusEffect statusEffect)
        {
            Name = statusEffect.Definition.Name;
            Icon = statusEffect.Definition.Icon;
            Duration = statusEffect.Definition.Duration;
            RemainingDuration = statusEffect.RemainingDuration;
            IsEndless = statusEffect.Definition.IsEndless;
        }
    }
}
