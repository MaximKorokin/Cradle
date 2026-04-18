using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Configs;
using Assets.CoreScripts;

namespace Assets._Game.Scripts.Entities.Stats
{
    public sealed class StatTickController
    {
        private readonly CooldownCounter _cooldownCounter;

        public StatTickController(StatsConfig statsConfig)
        {
            _cooldownCounter = new(1 / statsConfig.TickRate);
        }

        public void Tick(Entity entity)
        {
            var stats = entity.GetModule<StatModule>().Stats;
            var healthModule = entity.GetModule<HealthModule>();

            // HP regeneration
            if (_cooldownCounter.TryReset() && stats.Has(StatId.HpRegeneration))
            {
                var hpRegeneration = stats.Get(StatId.HpRegeneration);

                healthModule.Heal(hpRegeneration * _cooldownCounter.Cooldown);
            }
        }
    }
}
