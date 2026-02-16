using Assets._Game.Scripts.Infrastructure;
using Assets.CoreScripts;
using System;

namespace Assets._Game.Scripts.Entities.Stats
{
    public sealed class StatsTickController : IDisposable
    {
        private readonly StatsController _statsController;
        private readonly Dispatcher _dispatcher;

        private readonly CooldownCounter _cooldownCounter;

        public StatsTickController(StatsController statsController, StatsConfig statsConfig, Dispatcher dispatcher)
        {
            _statsController = statsController;
            _dispatcher = dispatcher;

            _cooldownCounter = new(1 / statsConfig.TickRate);

            _dispatcher.OnTick += OnTick;
        }

        private void OnTick()
        {
            // HP regeneration
            if (_cooldownCounter.TryReset() &&
                _statsController.Has(StatId.HpCurrent) &&
                _statsController.Has(StatId.HpRegeneration))
            {
                var hpCurrent = _statsController.Get(StatId.HpCurrent);
                var hpRegeneration = _statsController.Get(StatId.HpRegeneration);
                var hpMax = _statsController.Get(StatId.HpMax);

                if (hpCurrent >= hpMax) return;

                _statsController.SetBase(StatId.HpCurrent, hpCurrent + hpRegeneration * _cooldownCounter.Cooldown);
            }
        }

        public void Dispose()
        {
            _dispatcher.OnTick -= OnTick;
        }
    }
}
