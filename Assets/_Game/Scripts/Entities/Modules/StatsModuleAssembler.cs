using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Infrastructure;
using Assets._Game.Scripts.Infrastructure.Persistence;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class StatsModuleAssembler
    {
        private readonly StatsControllerAssembler _statsControllerAssembler;
        private readonly StatsConfig _statsConfig;
        private readonly DispatcherService _dispatcher;

        public StatsModuleAssembler(StatsControllerAssembler statsControllerAssembler, StatsConfig statsConfig, DispatcherService dispatcher)
        {
            _statsControllerAssembler = statsControllerAssembler;
            _statsConfig = statsConfig;
            _dispatcher = dispatcher;
        }

        public StatModule Create(EntityDefinition entityDefinition)
        {
            if (entityDefinition.TryGetModule<StatsModuleDefinition>(out var statsDefinitionModule))
            {
                var statsController = _statsControllerAssembler.Create(statsDefinitionModule.Stats);
                var statsTickController = new StatsTickController(statsController, _statsConfig, _dispatcher);
                return new(statsController, statsTickController);
            }
            return null;
        }

        public void Apply(StatModule statsModule, StatsSave statsSave)
        {
            _statsControllerAssembler.Apply(statsModule.Stats as StatsController, statsSave);
        }
    }
}
