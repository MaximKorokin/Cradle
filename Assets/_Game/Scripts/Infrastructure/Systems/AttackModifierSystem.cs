using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Interactions.Steps;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Querying;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class AttackModifierSystem : EntitySystemBase
    {
        private readonly IGlobalEventBus _globalEventBus;

        protected override EntityQuery EntityQuery => new(RestrictionState.Disabled);

        public AttackModifierSystem(EntityRepository repository, IGlobalEventBus globalEventBus) : base(repository)
        {
            _globalEventBus = globalEventBus;

            _globalEventBus.Subscribe<DamageAppliedEvent>(OnDamageApplied);
            TrackEntityEvent<StatusEffectChangedEvent>(OnStatusEffectChanged);
        }

        public override void Dispose()
        {
            base.Dispose();

            _globalEventBus.Unsubscribe<DamageAppliedEvent>(OnDamageApplied);
        }

        private void OnDamageApplied(DamageAppliedEvent e)
        {
            if (e.SourceType != DamageSourceType.Action) return;
            if (!e.Source.TryGetModule<AttackModifierModule>(out var attackModifier) ||
                !e.Source.TryGetModule<StatModule>(out var sourceStatModule) ||
                !e.Target.TryGetModule<StatModule>(out var targetStatModule)) return;

            foreach (var modifier in attackModifier.Modifiers)
            {
                if (Random.value > modifier.Chance) continue;

                switch (modifier.Type)
                {
                    case AttackModifierType.Vampiric:
                        if (modifier.Value <= 0) continue;
                        var healAmount = e.Damage * modifier.Value;
                        sourceStatModule.AddBase(StatId.HpCurrent, healAmount);
                        break;

                    case AttackModifierType.MultipliedRepeatDamage:
                        var extraDamage = e.Damage * modifier.Value;
                        targetStatModule.AddBase(StatId.HpCurrent, -extraDamage);
                        _globalEventBus.Publish(new DamageAppliedEvent(e.Target, e.Source, extraDamage, DamageSourceType.AttackModifier));
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
