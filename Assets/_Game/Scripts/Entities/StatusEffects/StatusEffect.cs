using Assets.CoreScripts;
using System;

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

        public Action<StatusEffect> Changed;

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
            if (ChargesAmount == charges) return;

            if (!Definition.Behaviour.HasFlag(StatusEffectBehaviour.Charges))
                SLog.Error("Status effect does not have charges behaviour.");

            ChargesAmount = charges;

            Changed?.Invoke(this);
        }
    }

    public readonly struct StatusEffectSnapshot
    {
        public readonly StatusEffectDefinition Definition;
        public readonly int RemainingChargesAmount;
        public readonly float RemainingDuration;

        public StatusEffectSnapshot(StatusEffectDefinition definition)
        {
            Definition = definition;
            RemainingChargesAmount = 0;
            RemainingDuration = 0;
        }

        public StatusEffectSnapshot(StatusEffect statusEffect)
        {
            Definition = statusEffect.Definition;
            RemainingChargesAmount = statusEffect.ChargesAmount;
            RemainingDuration = statusEffect.RemainingDuration;
        }
    }
}
