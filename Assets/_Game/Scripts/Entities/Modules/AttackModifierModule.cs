using Assets._Game.Scripts.Shared.Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class AttackModifierModule : EntityModuleBase
    {
        private readonly List<AttackModifierDefinition> _modifiers = new();

        public IReadOnlyList<AttackModifierDefinition> Modifiers => _modifiers;

        public void AddModifier(AttackModifierDefinition definition)
        {
            _modifiers.Add(definition);
        }

        public void RemoveModifier(AttackModifierDefinition definition)
        {
            _modifiers.Remove(definition);
        }

        public void ClearModifiers()
        {
            _modifiers.Clear();
        }
    }

    public enum AttackModifierType
    {
        None = 0,
        Vampiric = 10,
        [EnumDescription("Bonus Damage")]
        MultipliedRepeatDamage = 20,
        Chilling = 30,
        Heating = 40,
    }

    [Serializable]
    public sealed class AttackModifierDefinition
    {
        [field: SerializeField]
        public AttackModifierType Type { get; private set; }
        [field: SerializeField]
        public float Value { get; private set; }
        [field: SerializeField]
        public float Chance { get; private set; }
    }
}
