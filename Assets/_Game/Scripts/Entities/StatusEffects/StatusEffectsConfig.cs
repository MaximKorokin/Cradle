using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.StatusEffects
{
    [CreateAssetMenu(fileName = "StatusEffectsConfig", menuName = "ScriptableObjects/StatusEffectsConfig")]
    public sealed class StatusEffectsConfig : ScriptableObject
    {
        /// <summary>
        /// Number of update ticks per second.
        /// </summary>
        [field: SerializeField]
        public float TickRate { get; private set; }
        [field: SerializeField]
        public StatusEffectCategoryConfig[] Categories { get; private set; }

        private Dictionary<StatusEffectCategory, int> _maxAmountPerCategory = new();

        private void OnEnable()
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
    }

    [Serializable]
    public struct StatusEffectCategoryConfig
    {
        public StatusEffectCategory StatusEffectCategory;
        public int MaxAmount;
    }
}
