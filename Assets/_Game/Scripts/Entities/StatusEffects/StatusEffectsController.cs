using System;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Entities.StatusEffects
{
    public sealed class StatusEffectsController
    {
        private readonly Dictionary<StatusEffectCategory, List<StatusEffect>> _statusEffects = new();
        private readonly StatusEffectsConfig _config;

        public Action<StatusEffectChange> StatusEffectChanged;

        public StatusEffectsController(StatusEffectsConfig config)
        {
            _config = config;
        }

        public bool AddStatusEffect(StatusEffect statusEffect)
        {
            var category = statusEffect.Definition.Category;
            var maxAmount = _config.GetMaxAmountForCategory(category);
            if (maxAmount == 0) return false;

            if (!_statusEffects.TryGetValue(category, out var stack))
            {
                _statusEffects[category] = stack = new List<StatusEffect>();
            }

            if (stack.Count > _config.GetMaxAmountForCategory(category))
            {
                RemoveStatusEffect(stack[0]);
            }

            stack.Add(statusEffect);
            StatusEffectChanged?.Invoke(new StatusEffectChange
            {
                StatusEffect = statusEffect,
                Kind = StatusEffectChangeKind.Added
            });
            return true;
        }

        public void RemoveStatusEffect(StatusEffect statusEffect)
        {
            var category = statusEffect.Definition.Category;
            if (_statusEffects.TryGetValue(category, out var stack))
            {
                stack.Remove(statusEffect);
                StatusEffectChanged?.Invoke(new StatusEffectChange
                {
                    StatusEffect = statusEffect,
                    Kind = StatusEffectChangeKind.Removed
                });
            }
        }

        public void GetStatusEffectsForCategory(StatusEffectCategory category, List<StatusEffect> result)
        {
            if (_statusEffects.TryGetValue(category, out var stack))
            {
                result.AddRange(stack);
            }
        }
    }

    public struct StatusEffectChange
    {
        public StatusEffect StatusEffect;
        public StatusEffectChangeKind Kind;
    }

    public enum StatusEffectChangeKind
    {
        Added,
        Removed
    }
}
