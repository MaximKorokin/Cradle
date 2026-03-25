using Assets.CoreScripts;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.StatusEffects
{
    public sealed class StatusEffect
    {
        public StatusEffectDefinition Definition { get; }
        public float RemainingDuration => !Definition.Behaviour.HasFlag(StatusEffectBehaviour.Duration) ? float.PositiveInfinity : _cooldownCounter.Cooldown - _cooldownCounter.TimeSinceReset;
        public int ChargesAmount { get; private set; }
        public StatusEffectSnapshot Snapshot => new(this);

        private readonly CooldownCounter _cooldownCounter;

        public bool IsExpired => Definition.Behaviour.HasFlag(StatusEffectBehaviour.Duration) && _cooldownCounter.IsOver();

        public StatusEffect(StatusEffectDefinition definition)
        {
            Definition = definition;

            if (definition.Behaviour.HasFlag(StatusEffectBehaviour.Duration))
            {
                if (definition.Duration <= 0)
                    SLog.Error("StatusEffect: Duration must be greater than 0 for status effect with Duration behaviour.");
                _cooldownCounter = new(definition.Duration);
                _cooldownCounter.Reset();
            }

            if (definition.Behaviour.HasFlag(StatusEffectBehaviour.Charges))
            {
                if (definition.Charges <= 0)
                    SLog.Error("StatusEffect: Charges must be greater than 0 for status effect with Charges behaviour.");
                ChargesAmount = definition.Charges;
            }
        }

        public void SetCharges(int charges)
        {
            if (!Definition.Behaviour.HasFlag(StatusEffectBehaviour.Charges))
                SLog.Error("Status effect does not have charges behaviour.");
            ChargesAmount = charges;
        }
    }

    public readonly struct StatusEffectSnapshot
    {
        public readonly string Name;
        public readonly Sprite Icon;
        public readonly float Duration;
        public readonly float RemainingDuration;
        public readonly StatusEffectBehaviour Behaviour;

        public StatusEffectSnapshot(StatusEffect statusEffect)
        {
            Name = statusEffect.Definition.Name;
            Icon = statusEffect.Definition.Icon;
            Duration = statusEffect.Definition.Duration;
            RemainingDuration = statusEffect.RemainingDuration;
            Behaviour = statusEffect.Definition.Behaviour;
        }
    }
}
