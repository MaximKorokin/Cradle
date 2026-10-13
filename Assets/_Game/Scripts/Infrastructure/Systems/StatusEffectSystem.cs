using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.Infrastructure.Configs;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Querying;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Traits;
using Assets._Game.Scripts.Shared.Extensions;
using Assets.CoreScripts;
using System.Linq;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class StatusEffectSystem : EntitySystemBase, ITickSystem
    {
        private readonly CooldownCounter _tickCooldownCounter;

        protected override EntityQuery EntityQuery { get; } = new(RestrictionState.Disabled, new[] { typeof(StatusEffectModule) });

        public StatusEffectSystem(
            IGlobalEventBus globalEventBus,
            EntityRepository repository,
            StatusEffectsConfig statusEffectsConfig) : base(globalEventBus, repository)
        {
            _tickCooldownCounter = new(1 / statusEffectsConfig.TickRate);

            TrackEntityEvent<ActionCompletedEvent>(OnActionCompleted);
            TrackEntityEvent<ItemUseStartedEvent>(OnItemUseStarted);

            TrackGlobalEvent<EntityDiedEvent>(OnEntityDied);
            TrackGlobalEvent<StatusEffectApplyRequest>(OnStatusEffectApplyRequested);
            TrackGlobalEvent<StatusEffectRemoveRequest>(OnStatusEffectRemoveRequested);
            TrackGlobalEvent<StatusEffectsClearRequest>(OnStatusEffectsClearRequested);
        }

        public void Tick(float delta)
        {
            IterateMatchingEntities(Tick);
        }

        public void Tick(Entity entity)
        {
            var statusEffectModule = entity.GetModule<StatusEffectModule>();

            if (_tickCooldownCounter.TryReset())
            {
                var statusEffects = statusEffectModule.StatusEffects.GetStatusEffects().ToArray();
                foreach (var statusEffect in statusEffects)
                {
                    if (statusEffect.IsExpired)
                    {
                        GlobalEventBus.Publish(new StatusEffectRemoveRequest(entity, statusEffect));
                    }
                }
            }
        }

        private void OnActionCompleted(Entity entity, ActionCompletedEvent e)
        {
            if (!e.ActionInstance.Definition.ConsumesCharges) return;
            if (!entity.TryGetModule<StatusEffectModule>(out var statusEffectModule)) return;

            var statusEffects = statusEffectModule.StatusEffects.GetStatusEffects().ToArray();
            foreach (var statusEffect in statusEffects)
            {
                if (statusEffect.Definition.Behaviour.HasFlag(StatusEffectBehaviour.Charges))
                {
                    statusEffect.SetCharges(statusEffect.ChargesAmount - 1);
                    if (statusEffect.ChargesAmount <= 0)
                    {
                        GlobalEventBus.Publish(new StatusEffectRemoveRequest(entity, statusEffect));
                    }
                }
            }
        }

        private void OnItemUseStarted(Entity entity, ItemUseStartedEvent e)
        {
            if (!entity.TryGetModule<StatusEffectModule>(out var statusEffectModule)) return;

            foreach (var trait in e.Item.GetFunctionalTraits<StatusEffectTrait>(ItemTrigger.OnUse))
            {
                // No need to check if trait can trigger, as it was already checked in ItemSystem when starting item use.
                GlobalEventBus.Publish(new StatusEffectApplyRequest(entity, new(trait.StatusEffect)));
            }
        }

        private void OnStatusEffectApplyRequested(StatusEffectApplyRequest e)
        {
            if (!e.Target.TryGetModule<StatusEffectModule>(out var statusEffectModule)) return;

            statusEffectModule.StatusEffects.AddStatusEffect(e.StatusEffect);
        }

        private void OnEntityDied(EntityDiedEvent e)
        {
            GlobalEventBus.Publish(new StatusEffectsClearRequest(e.Victim));
        }

        private void OnStatusEffectRemoveRequested(StatusEffectRemoveRequest e)
        {
            if (!e.Target.TryGetModule<StatusEffectModule>(out var statusEffectModule)) return;

            statusEffectModule.StatusEffects.RemoveStatusEffect(e.StatusEffect);
        }

        private void OnStatusEffectsClearRequested(StatusEffectsClearRequest e)
        {
            if (!e.Target.TryGetModule<StatusEffectModule>(out var statusEffectModule)) return;

            statusEffectModule.StatusEffects.ClearStatusEffects();
        }
    }

    public readonly struct StatusEffectRemoveRequest : IGlobalEvent
    {
        public readonly Entity Target;
        public readonly StatusEffect StatusEffect;

        public StatusEffectRemoveRequest(Entity target, StatusEffect statusEffect)
        {
            Target = target;
            StatusEffect = statusEffect;
        }
    }

    public readonly struct StatusEffectsClearRequest : IGlobalEvent
    {
        public readonly Entity Target;

        public StatusEffectsClearRequest(Entity target)
        {
            Target = target;
        }
    }

    public readonly struct StatusEffectApplyRequest : IGlobalEvent
    {
        public readonly Entity Target;
        public readonly StatusEffect StatusEffect;

        public StatusEffectApplyRequest(Entity target, StatusEffect statusEffect)
        {
            Target = target;
            StatusEffect = statusEffect;
        }
    }
}
