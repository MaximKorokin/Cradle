using Assets._Game.Scripts.Infrastructure;
using Assets.CoreScripts;
using System;
using System.Linq;

namespace Assets._Game.Scripts.Entities.StatusEffects
{
    public sealed class StatusEffectsTickController
    {
        private readonly StatusEffectsController _statusEffectsController;
        private readonly Dispatcher _dispatcher;

        private readonly CooldownCounter _cooldownCounter;

        public StatusEffectsTickController(StatusEffectsController statusEffectsController, StatusEffectsConfig statusEffectsConfig, Dispatcher dispatcher)
        {
            _statusEffectsController = statusEffectsController;
            _dispatcher = dispatcher;

            _cooldownCounter = new(1 / statusEffectsConfig.TickRate);

            _dispatcher.OnTick += OnTick;
        }

        private void OnTick()
        {
            if (_cooldownCounter.TryReset())
            {
                var statusEffects = Enum.GetValues(typeof(StatusEffectCategory))
                    .Cast<StatusEffectCategory>()
                    .SelectMany(c => _statusEffectsController.GetStatusEffectsForCategory(c))
                    .ToArray();
                foreach (var statusEffect in statusEffects)
                {
                    statusEffect.OnTick();
                }
            }
        }

        public void Dispose()
        {
            _dispatcher.OnTick -= OnTick;
        }
    }
}
