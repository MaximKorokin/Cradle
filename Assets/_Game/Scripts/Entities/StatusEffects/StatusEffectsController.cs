using Assets._Game.Scripts.Infrastructure.Configs;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public StatusEffect GetStatusEffect(StatusEffectDefinition statusEffectDefinition)
        {
            if (!_statusEffects.TryGetValue(statusEffectDefinition.Category, out var stack)) return null;
            return stack.FirstOrDefault(se => se.Definition.Id == statusEffectDefinition.Id);
        }

        public bool HasStatusEffect(StatusEffectDefinition statusEffectDefinition)
        {
            return GetStatusEffect(statusEffectDefinition) != null;
        }

        public bool AddStatusEffect(StatusEffectDefinition statusEffectDefinition)
        {
            var category = statusEffectDefinition.Category;
            var maxAmount = _config.GetMaxAmountForCategory(category);
            if (maxAmount == 0) return false;

            if (!_statusEffects.TryGetValue(category, out var stack))
            {
                _statusEffects[category] = stack = new List<StatusEffect>();
            }

            if (HasStatusEffect(statusEffectDefinition))
            {
                RemoveStatusEffect(statusEffectDefinition);
            }

            if (stack.Count >= maxAmount)
            {
                RemoveStatusEffect(stack[0].Definition);
            }

            var statusEffect = new StatusEffect(statusEffectDefinition);
            statusEffect.Changed += OnStatusEffectChanged;
            stack.Add(statusEffect);
            StatusEffectChanged?.Invoke(new StatusEffectChange
            {
                StatusEffect = statusEffect,
                Kind = StatusEffectChangeKind.Added
            });

            Changed?.Invoke();
            return true;
        }

        public bool WouldRemoveOtherStatusEffect(StatusEffectDefinition definition)
        {
            var category = definition.Category;
            var maxAmount = _config.GetMaxAmountForCategory(category);
            if (maxAmount == 0) return false;
            if (!_statusEffects.TryGetValue(category, out var stack)) return false;
            if (stack.Any(se => se.Definition.Id == definition.Id)) return false;
            return stack.Count >= maxAmount;
        }

        public void RemoveStatusEffect(StatusEffectDefinition statusEffectDefinition)
        {
            var category = statusEffectDefinition.Category;
            var statusEffect = GetStatusEffect(statusEffectDefinition);
            if (_statusEffects.TryGetValue(category, out var stack) && stack.Remove(statusEffect))
            {
                statusEffect.Changed -= OnStatusEffectChanged;
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
                    RemoveStatusEffect(statusEffect.Definition);
                }
            }
        }

        private void OnStatusEffectChanged(StatusEffect statusEffect)
        {
            StatusEffectChanged?.Invoke(new StatusEffectChange
            {
                StatusEffect = statusEffect,
                Kind = StatusEffectChangeKind.Updated
            });
            Changed?.Invoke();
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
        Removed,
        Updated,
    }
}
