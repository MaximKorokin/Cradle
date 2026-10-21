using Assets._Game.Scripts.Entities.StatusEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Configs
{
    [CreateAssetMenu(menuName = "Configs/StatusEffectsConfig")]
    public sealed class StatusEffectsConfig : ScriptableObject
    {
        /// <summary>
        /// Number of update ticks per second.
        /// </summary>
        [field: SerializeField]
        public float TickRate { get; private set; }
        [field: SerializeField]
        public StatusEffectCategoryConfig[] Categories { get; private set; }
        [field: SerializeField]
        public SpecialStatusEffectDefinition[] SpecialStatusEffects { get; private set; }

        private Dictionary<StatusEffectCategory, int> _maxAmountPerCategory = new();

        private void OnValidate()
        {
            if (Categories != null) _maxAmountPerCategory = Categories.ToDictionary(c => c.StatusEffectCategory, c => c.MaxAmount);
        }

        public int GetMaxAmountForCategory(StatusEffectCategory category)
        {
            if (_maxAmountPerCategory.TryGetValue(category, out var amount))
            {
                return amount;
            }
            return 0;
        }

        public StatusEffectDefinition GetSpecialStatusEffect(SpecialStatusEffectKind kind)
        {
            return SpecialStatusEffects.FirstOrDefault(x => x.Kind == kind).StatusEffectDefinition;
        }
    }

    [Serializable]
    public struct StatusEffectCategoryConfig
    {
        public StatusEffectCategory StatusEffectCategory;
        public int MaxAmount;
    }

    [Serializable]
    public struct SpecialStatusEffectDefinition
    {
        public SpecialStatusEffectKind Kind;
        public StatusEffectDefinition StatusEffectDefinition;
    }

    public enum SpecialStatusEffectKind
    {
        None = 0,
        Overweight = 10,
    }
}
