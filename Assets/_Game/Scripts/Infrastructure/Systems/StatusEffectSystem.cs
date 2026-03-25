using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.Infrastructure.Configs;
using Assets._Game.Scripts.Infrastructure.Querying;
using Assets.CoreScripts;
using System.Linq;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class StatusEffectSystem : EntitySystemBase, ITickSystem
    {
        private readonly CooldownCounter _cooldownCounter;

        protected override EntityQuery EntityQuery => new(RestrictionState.Disabled, new[] { typeof(StatusEffectModule) });

        public StatusEffectSystem(EntityRepository repository, StatusEffectsConfig statusEffectsConfig) : base(repository)
        {
            _cooldownCounter = new(1 / statusEffectsConfig.TickRate);

            TrackEntityEvent<ActionCompletedEvent>(OnActionCompleted);
        }

        public void Tick(float delta)
        {
            IterateMatchingEntities(Tick);
        }

        public void Tick(Entity entity)
        {
            var statusEffectModule = entity.GetModule<StatusEffectModule>();

            if (_cooldownCounter.TryReset())
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
            var entity = e.Entity;
            if (!entity.TryGetModule<StatusEffectModule>(out var statusEffectModule)) return;
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
    }
}
