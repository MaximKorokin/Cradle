using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.Infrastructure.Configs;
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

        protected override EntityQuery EntityQuery => new(RestrictionState.Disabled, new[] { typeof(StatusEffectModule) });

        public StatusEffectSystem(EntityRepository repository, StatusEffectsConfig statusEffectsConfig) : base(repository)
        {
            _tickCooldownCounter = new(1 / statusEffectsConfig.TickRate);

            TrackEntityEvent<ActionCompletedEvent>(OnActionCompleted);
            TrackEntityEvent<ItemUseStartedEvent>(OnItemUseStarted);
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
                        statusEffectModule.StatusEffects.RemoveStatusEffect(statusEffect);
                    }
                }
            }
        }

        private void OnActionCompleted(ActionCompletedEvent e)
        {
            if (!e.ActionInstance.Definition.ConsumesCharges) return;
            if (!e.Entity.TryGetModule<StatusEffectModule>(out var statusEffectModule)) return;

            var statusEffects = statusEffectModule.StatusEffects.GetStatusEffects().ToArray();
            foreach (var statusEffect in statusEffects)
            {
                if (statusEffect.Definition.Behaviour.HasFlag(StatusEffectBehaviour.Charges))
                {
                    statusEffect.SetCharges(statusEffect.ChargesAmount - 1);
                    if (statusEffect.ChargesAmount <= 0)
                    {
                        statusEffectModule.StatusEffects.RemoveStatusEffect(statusEffect);
                    }
                }
            }
        }

        private void OnItemUseStarted(ItemUseStartedEvent e)
        {
            if (!e.Entity.TryGetModule<StatusEffectModule>(out var statusEffectModule)) return;

            foreach (var trait in e.Item.GetFunctionalTraits<StatusEffectTrait>(e.IsManual ? ItemTrigger.OnManualUse : ItemTrigger.OnAutoUse))
            {
                statusEffectModule.StatusEffects.AddStatusEffect(new(trait.StatusEffect));
            }
        }
    }
}
