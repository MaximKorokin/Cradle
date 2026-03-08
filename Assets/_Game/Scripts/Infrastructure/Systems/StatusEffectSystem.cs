using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.Shared.Extensions;
using Assets.CoreScripts;
using System;
using System.Linq;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class StatusEffectSystem : EntitySystemBase
    {
        private readonly CooldownCounter _cooldownCounter;

        protected override EntityQuery EntityQuery => new(RestrictionState.Disabled);

        public StatusEffectSystem(EntityRepository repository, DispatcherService dispatcher, StatusEffectsConfig statusEffectsConfig) : base(repository, dispatcher)
        {
            _cooldownCounter = new(1 / statusEffectsConfig.TickRate);
            TickAction += (e, _) => Tick(e);
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

        protected override bool Filter(Entity entity)
        {
            return entity.HasModule<StatusEffectModule>();
        }
    }
}
