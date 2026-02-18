using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.Infrastructure;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class StatusEffectModuleAssembler
    {
        private readonly StatusEffectsConfig _config;
        private readonly DispatcherService _dispatcher;

        public StatusEffectModuleAssembler(StatusEffectsConfig config, DispatcherService dispatcher)
        {
            _config = config;
            _dispatcher = dispatcher;
        }

        public StatusEffectModule Assemble()
        {
            var controller = new StatusEffectsController(_config);
            var tickController = new StatusEffectsTickController(controller, _config, _dispatcher);
            var module = new StatusEffectModule(controller, tickController);
            return module;
        }
    }
}
