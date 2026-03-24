using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class AttackModifierModule : EntityModuleBase
    {
        private readonly List<AttackModifierEntry> _modifiers = new();

        public IReadOnlyList<AttackModifierEntry> Modifiers => _modifiers;

        public void AddModifier(AttackModifierEntry attackModifierEntry)
        {
            _modifiers.Add(attackModifierEntry);
        }

        public void RemoveModifier(AttackModifierEntry attackModifierEntry)
        {
            _modifiers.Remove(attackModifierEntry);
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
        MultipliedDamage = 20,
        Chilling = 30,
        Heating = 40,
    }

    public readonly struct AttackModifierEntry
    {
        public readonly AttackModifierType Type;
        public readonly float Value;
        public readonly float Chance;

        public AttackModifierEntry(AttackModifierType type, float value, float chance)
        {
            Type = type;
            Value = value;
            Chance = chance;
        }
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

        public AttackModifierEntry CreateInstance()
        {
            return new AttackModifierEntry(Type, Value, Chance);
        }
    }
}
