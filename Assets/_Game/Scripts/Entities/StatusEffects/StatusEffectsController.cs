using System;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Entities.StatusEffects
{
    public sealed class StatusEffectsController
    {
        private readonly Dictionary<StatusEffectCategory, List<StatusEffect>> _statusEffects = new();
        private readonly StatusEffectsConfig _config;

        public event Action<StatusEffectChange> StatusEffectChanged;
        public event Action Changed;

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
            Changed?.Invoke();
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
                Changed?.Invoke();
            }
        }

        public IEnumerable<StatusEffect> GetStatusEffectsForCategory(StatusEffectCategory category)
        {
            if (_statusEffects.TryGetValue(category, out var stack))
            {
                return stack.ToArray();
            }
            return null;
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
