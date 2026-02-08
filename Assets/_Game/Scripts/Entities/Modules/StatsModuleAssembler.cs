using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Infrastructure.Persistence;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class StatsModuleAssembler
    {
        private readonly StatsControllerAssembler _statsControllerAssembler;

        public StatsModuleAssembler(StatsControllerAssembler statsControllerAssembler)
        {
            _statsControllerAssembler = statsControllerAssembler;
        }

        public StatsModule Create()
        {
            return new(_statsControllerAssembler.Create());
        }

        public void Apply(StatsModule statsModule, StatsSave statsSave)
        {
            _statsControllerAssembler.Apply(statsModule.Stats as StatsController, statsSave);
        }
    }
}
