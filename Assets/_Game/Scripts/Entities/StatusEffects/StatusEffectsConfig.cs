using System.Collections.Generic;
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
        public int MaxBuffsAmount { get; private set; }
        [field: SerializeField]
        public int MaxDebuffsAmount { get; private set; }

        private readonly Dictionary<StatusEffectCategory, int> _maxAmountPerCategory = new();

        private void OnEnable()
        {
            _maxAmountPerCategory[StatusEffectCategory.Buff] = MaxBuffsAmount;
            _maxAmountPerCategory[StatusEffectCategory.Debuff] = MaxDebuffsAmount;
        }

        public int GetMaxAmountForCategory(StatusEffectCategory category)
        {
            if (_maxAmountPerCategory.TryGetValue(category, out var amount))
            {
                return amount;
            }
            return MaxBuffsAmount;
        }
    }
}
