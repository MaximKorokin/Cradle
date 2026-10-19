using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.Infrastructure.Configs;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Querying;
using Assets._Game.Scripts.Infrastructure.Systems.GameAnalytics;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Traits;
using Assets._Game.Scripts.Shared.Extensions;
using Assets.CoreScripts;
using System.Linq;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class StatusEffectSystem : EntitySystemBase, ITickSystem
    {
        private readonly StatusEffectsConfig _statusEffectsConfig;
        private readonly CooldownCounter _tickCooldownCounter;

        protected override EntityQuery EntityQuery { get; } = new(RestrictionState.Disabled, new[] { typeof(StatusEffectModule) });

        public StatusEffectSystem(
            IGlobalEventBus globalEventBus,
            EntityRepository repository,
            StatusEffectsConfig statusEffectsConfig) : base(globalEventBus, repository)
        {
            _statusEffectsConfig = statusEffectsConfig;
            _tickCooldownCounter = new(1 / statusEffectsConfig.TickRate);

            TrackEntityEvent<ActionCompletedEvent>(OnActionCompleted);
            TrackEntityEvent<ItemUseStartedEvent>(OnItemUseStarted);
            TrackEntityEvent<StatChangedEvent>(OnStatChanged);

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
                        GlobalEventBus.Publish(new StatusEffectRemoveRequest(entity, statusEffect.Definition));
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
                        GlobalEventBus.Publish(new StatusEffectRemoveRequest(entity, statusEffect.Definition));
                    }
                }
            }
        }

        private void OnItemUseStarted(Entity entity, ItemUseStartedEvent e)
        {
            foreach (var trait in e.Item.GetFunctionalTraits<StatusEffectTrait>(ItemTrigger.OnUse))
            {
                // No need to check if trait can trigger, as it was already checked in ItemSystem when starting item use.
                GlobalEventBus.Publish(new StatusEffectApplyRequest(entity, entity, trait.StatusEffect));
            }
        }

        private void OnStatChanged(Entity entity, StatChangedEvent e)
        {
            if ((e.StatId != StatId.CarryWeight &&
                e.StatId != StatId.CarryWeightMax) ||
                !entity.TryGetModule<StatModule>(out var statsModule) ||
                !entity.TryGetModule<StatusEffectModule>(out var statusEffectModule)) return;

            var overweightStatusEffectDefinition = _statusEffectsConfig.GetSpecialStatusEffect(SpecialStatusEffectKind.Overweight);

            var carryWeight = statsModule.Stats.Get(StatId.CarryWeight);
            var carryWeightMax = statsModule.Stats.Get(StatId.CarryWeightMax);

            if (carryWeight > carryWeightMax)
            {
                if (!statusEffectModule.StatusEffects.HasStatusEffect(overweightStatusEffectDefinition))
                    GlobalEventBus.Publish(new StatusEffectApplyRequest(entity, entity, overweightStatusEffectDefinition));
            }
            else if (statusEffectModule.StatusEffects.HasStatusEffect(overweightStatusEffectDefinition))
            {
                GlobalEventBus.Publish(new StatusEffectRemoveRequest(entity, overweightStatusEffectDefinition));
            }

        }

        private void OnStatusEffectApplyRequested(StatusEffectApplyRequest e)
        {
            if (!e.Target.TryGetModule<StatusEffectModule>(out var statusEffectModule)) return;

            if (statusEffectModule.StatusEffects.AddStatusEffect(e.StatusEffectDefinition))
                GlobalEventBus.Publish(new GameAnalyticsEvent(AnalyticsEntryType.StatusEffectStart, null, e.Target, 0f, e.StatusEffectDefinition.Name));
        }

        private void OnEntityDied(EntityDiedEvent e)
        {
            GlobalEventBus.Publish(new StatusEffectsClearRequest(e.Victim));
        }

        private void OnStatusEffectRemoveRequested(StatusEffectRemoveRequest e)
        {
            if (!e.Target.TryGetModule<StatusEffectModule>(out var statusEffectModule)) return;

            statusEffectModule.StatusEffects.RemoveStatusEffect(e.StatusEffectDefinition);
            GlobalEventBus.Publish(new GameAnalyticsEvent(AnalyticsEntryType.StatusEffectEnd, null, e.Target, 0f, e.StatusEffectDefinition.Name));
        }

        private void OnStatusEffectsClearRequested(StatusEffectsClearRequest e)
        {
            if (!e.Target.TryGetModule<StatusEffectModule>(out var statusEffectModule)) return;

            foreach (var statusEffect in statusEffectModule.StatusEffects.GetStatusEffects())
                GlobalEventBus.Publish(new GameAnalyticsEvent(AnalyticsEntryType.StatusEffectEnd, null, e.Target, 0f, statusEffect.Definition.name));

            statusEffectModule.StatusEffects.ClearStatusEffects();
        }
    }

    public readonly struct StatusEffectRemoveRequest : IGlobalEvent
    {
        public readonly Entity Target;
        public readonly StatusEffectDefinition StatusEffectDefinition;

        public StatusEffectRemoveRequest(Entity target, StatusEffectDefinition statusEffectDefinition)
        {
            Target = target;
            StatusEffectDefinition = statusEffectDefinition;
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
        public readonly Entity Source;
        public readonly Entity Target;
        public readonly StatusEffectDefinition StatusEffectDefinition;

        public StatusEffectApplyRequest(Entity source, Entity target, StatusEffectDefinition statusEffectDefinition)
        {
            Source = source;
            Target = target;
            StatusEffectDefinition = statusEffectDefinition;
        }
    }
}
