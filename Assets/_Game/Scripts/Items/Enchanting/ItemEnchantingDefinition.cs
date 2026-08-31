using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Shared;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Game.Scripts.Items.Enchanting
{
    [CreateAssetMenu(menuName = "Definitions/ItemEnchanting", fileName = "Enchanting")]
    public class ItemEnchantingDefinition : GuidScriptableObject
    {
        [SerializeField]
        private EnchantLevelDefinition[] _levels;

        [SerializeField]
        private EnchantMethodDefinition[] _methods;

        public IReadOnlyList<EnchantLevelDefinition> Levels => _levels;
        public IReadOnlyList<EnchantMethodDefinition> Methods => _methods;
    }

    [Serializable]
    public class EnchantLevelDefinition
    {
        [SerializeField]
        private StatModifier[] _bonuses;

        public IReadOnlyList<StatModifier> Bonuses => _bonuses;
    }

    [Serializable]
    public class EnchantMethodDefinition
    {
        [field: SerializeField]
        public ItemDefinition Item { get; private set; }

        [SerializeField]
        private EnchantLevelRuleDefinition[] _rules;

        public IReadOnlyList<EnchantLevelRuleDefinition> Rules => _rules;
    }

    [Serializable]
    public class EnchantLevelRuleDefinition
    {
        [field: SerializeField]
        [field: Range(0f, 1f)]
        public float SuccessChance { get; private set; }

        [field: SerializeField]
        public EnchantFailureDefinition Failure { get; private set; }
    }

    [Serializable]
    public class EnchantFailureDefinition
    {
        [field: SerializeField]
        public EnchantFailureType Type { get; private set; }
    }

    public enum EnchantFailureType
    {
        None,
        Downgrade,
        Reset,
        Destroy
    }
}
