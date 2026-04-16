using Assets._Game.Scripts.Infrastructure.Configs;
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

            StatusEffect activeStatusEffect = null;
            for (int i = 0; i < stack.Count; i++)
            {
                if (stack[i].Definition.Id == statusEffect.Definition.Id)
                {
                    activeStatusEffect = stack[i];
                    break;
                }
            }

            if (activeStatusEffect != null)
            {
                RemoveStatusEffect(activeStatusEffect);
            }

            if (stack.Count >= maxAmount)
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
                return stack;
            }
            return Array.Empty<StatusEffect>();
        }

        public IEnumerable<StatusEffect> GetStatusEffects()
        {
            foreach (var stack in _statusEffects.Values)
            {
                for (int i = 0; i < stack.Count; i++)
                {
                    yield return stack[i];
                }
            }
        }

        public void ClearStatusEffects()
        {
            foreach (var stack in _statusEffects.Values)
            {
                if (stack.Count == 0) continue;

                foreach (var statusEffect in stack.ToArray())
                {
                    RemoveStatusEffect(statusEffect);
                }
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
