using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.Infrastructure.Configs;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class StatusEffectModuleAssembler
    {
        private readonly StatusEffectsConfig _config;

        public StatusEffectModuleAssembler(StatusEffectsConfig config)
        {
            _config = config;
        }

        public StatusEffectModule Assemble()
        {
            var controller = new StatusEffectsController(_config);
            var module = new StatusEffectModule(controller);
            return module;
        }
    }
}
