using Assets._Game.Scripts.Entities.Stats;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class DerivedStatsApplierModule : EntityModuleBase
    {
        private static readonly object Source = new();

        private readonly StatsModule _statsModule;

        public DerivedStatsApplierModule(StatsModule statsModule)
        {
            _statsModule = statsModule;
        }

        protected override void OnAttach()
        {
            Subscribe<StatChangedEvent>(OnStatChanged);
            Rebuild();
        }

        private void OnStatChanged(StatChangedEvent e)
        {
            // todo: list primary stats at least in an array
            if (e.StatId == StatId.Strength || e.StatId == StatId.Agility)
                Rebuild();
        }

        private void Rebuild()
        {
            _statsModule.RemoveModifiers(Source);

            var strength = _statsModule.Stats.GetBase(StatId.Strength);
            var agility = _statsModule.Stats.GetBase(StatId.Agility);

            // todo: list formulas at least in an array
            _statsModule.AddModifiers(Source, new[]
            {
                new StatModifier(StatId.Damage, StatStage.PreAdd, StatOperation.Add, strength * 2f),
                new StatModifier(StatId.AttackSpeed, StatStage.PreAdd, StatOperation.Add, agility * 0.01f),
            });
        }
    }
}
