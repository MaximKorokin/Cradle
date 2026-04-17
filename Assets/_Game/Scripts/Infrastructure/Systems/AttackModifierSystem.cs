using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Interactions.Steps;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Querying;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class AttackModifierSystem : EntitySystemBase
    {
        protected override EntityQuery EntityQuery => new(RestrictionState.Disabled);

        public AttackModifierSystem(
            IGlobalEventBus globalEventBus,
            EntityRepository repository) : base(globalEventBus, repository)
        {
            TrackGlobalEvent<DamageAppliedEvent>(OnDamageApplied);
            TrackEntityEvent<StatusEffectChangedEvent>(OnStatusEffectChanged);
        }

        protected override void OnEntityAdded(Entity entity)
        {
            base.OnEntityAdded(entity);

            if (!EntityQuery.Match(entity)) return;

            if (entity.TryGetModule<StatusEffectModule>(out var statusEffectModule))
            {
                foreach (var statusEffect in statusEffectModule.StatusEffects.GetStatusEffects())
                {
                    OnStatusEffectChanged(entity, new StatusEffectChangedEvent(new() { Kind = StatusEffectChangeKind.Added, StatusEffect = statusEffect }));
                }
            }
        }

        private void OnDamageApplied(DamageAppliedEvent e)
        {
            if (e.SourceType != DamageSourceType.Action) return;
            if (!e.Source.TryGetModule<AttackModifierModule>(out var attackModifier) ||
                !e.Source.TryGetModule<HealthModule>(out var sourceHealthModule) ||
                !e.Target.TryGetModule<HealthModule>(out var targetHealthModule)) return;

            foreach (var modifier in attackModifier.Modifiers)
            {
                if (Random.value > modifier.Chance) continue;

                switch (modifier.Type)
                {
                    case AttackModifierType.Vampiric:
                        if (modifier.Value <= 0) continue;
                        var healAmount = e.Damage * modifier.Value;
                        sourceHealthModule.Heal(healAmount);
                        break;

                    case AttackModifierType.MultipliedRepeatDamage:
                        var extraDamage = e.Damage * modifier.Value;
                        var appliedDamage = targetHealthModule.ApplyDamage(extraDamage);
                        GlobalEventBus.Publish(new DamageAppliedEvent(e.Target, e.Source, appliedDamage, DamageSourceType.AttackModifier));
                        break;
                }
            }
        }

        private void OnStatusEffectChanged(Entity entity, StatusEffectChangedEvent e)
        {
            if (e.StatusEffect.Definition.AttackModifiers == null || e.StatusEffect.Definition.AttackModifiers.Length == 0) return;

            if (e.Kind == StatusEffectChangeKind.Added)
            {
                // If the entity doesn't have an AttackModifierModule, add one.
                if (!entity.TryGetModule<AttackModifierModule>(out var attackModifierModule))
                {
                    attackModifierModule = new();
                    entity.AddModule(attackModifierModule);
                }

                for (int i = 0; i < e.StatusEffect.Definition.AttackModifiers.Length; i++)
                {
                    attackModifierModule.AddModifier(e.StatusEffect.Definition.AttackModifiers[i]);
                }
            }
            else if (e.Kind == StatusEffectChangeKind.Removed)
            {
                if (!entity.TryGetModule<AttackModifierModule>(out var attackModifierModule)) return;
                for (int i = 0; i < e.StatusEffect.Definition.AttackModifiers.Length; i++)
                {
                    attackModifierModule.RemoveModifier(e.StatusEffect.Definition.AttackModifiers[i]);
                }
            }
        }
    }
}
