using Assets._Game.Scripts.Entities.Control;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Infrastructure.Storage;
using Assets._Game.Scripts.Shared.Attributes;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.StatusEffects
{
    [CreateAssetMenu(menuName = "Definitions/StatusEffect")]
    public sealed class StatusEffectDefinition : GuidScriptableObject
    {
        [field: SerializeField]
        public string Name { get; private set; }
        [field: SerializeField]
        public Sprite Icon { get; private set; }
        [field: SerializeField]
        public StatusEffectCategory Category { get; private set; }

        [field: Header("Lifetime Behaviour")]
        [field: SerializeField]
        public StatusEffectBehaviour Behaviour { get; private set; }
        [field: SerializeField]
        [field: ConditionalDisplay(nameof(Behaviour), StatusEffectBehaviour.Duration, ConditionalDisplayAttribute.ComparisonType.Flag)]
        public float Duration { get; private set; }
        [field: SerializeField]
        [field: ConditionalDisplay(nameof(Behaviour), StatusEffectBehaviour.Charges, ConditionalDisplayAttribute.ComparisonType.Flag)]
        public int Charges { get; private set; }
        [field: SerializeField]
        [field: ConditionalDisplay(nameof(Behaviour), StatusEffectBehaviour.Stacks, ConditionalDisplayAttribute.ComparisonType.Flag)]
        public int Stacks { get; private set; }

        [field: SerializeField]
        public StatModifier[] StatModifiers { get; private set; }
        [field: SerializeField]
        public AttackModifierDefinition[] AttackModifiers { get; private set; }
        [field: SerializeReference]
        [field: Header("Control Provider")]
        public ControlProviderData ControlProvider { get; private set; }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (ControlProvider is TimedControlProviderData timed)
            {
                timed.Duration = Duration;
            }
        }
    }

    public enum StatusEffectCategory
    {
        Buff,
        Debuff
    }

    [Flags]
    public enum StatusEffectBehaviour
    {
        None = 0,
        Duration = 1,
        Charges = 16,
        Stacks = 256,
    }
}
