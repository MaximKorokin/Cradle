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

            // HP regeneration
            if (_cooldownCounter.TryReset() &&
                stats.Has(StatId.HpCurrent) &&
                stats.Has(StatId.HpRegeneration))
            {
                var hpCurrent = stats.Get(StatId.HpCurrent);
                var hpRegeneration = stats.Get(StatId.HpRegeneration);
                var hpMax = stats.Get(StatId.HpMax);

                if (hpCurrent >= hpMax) return;

                stats.SetBase(StatId.HpCurrent, hpCurrent + hpRegeneration * _cooldownCounter.Cooldown);
            }
        }
    }
}
