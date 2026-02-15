using Assets._Game.Scripts.Entities.StatusEffects;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class StatusEffectModuleAssembler
    {
        private readonly StatusEffectsConfig _config;
        private readonly StatusEffectSystem _statusEffectSystem;

        public StatusEffectModuleAssembler(StatusEffectsConfig config, StatusEffectSystem statusEffectSystem)
        {
            _config = config;
        }

        public StatusEffectModule Assemble()
        {
            var module = new StatusEffectModule(_config);
            return module;
        }
    }
}
