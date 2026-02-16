using Assets._Game.Scripts.Entities.Stats;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Entities.Modules
{
    public class StatModule : EntityModuleBase
    {
        private readonly StatsController _statsController;
        private readonly StatsTickController _statsTickController;

        public StatModule(StatsController stats, StatsTickController statsTickController)
        {
            _statsController = stats;
            _statsTickController = statsTickController;

            _statsController.StatChanged += OnStatChanged;
            _statsController.Changed += OnStatsChanged;
        }

        public IStatsReadOnly Stats => _statsController;

        public override void Dispose()
        {
            base.Dispose();

            _statsTickController.Dispose();
        }

        public void AddModifiers(object source, IEnumerable<StatModifier> modifiers)
        {
            _statsController.AddModifiers(source, modifiers);
        }

        public void RemoveModifiers(object source)
        {
            _statsController.RemoveModifiers(source);
        }

        private void OnStatChanged(StatId id)
        {
            Publish<StatChangedEvent>(new());
        }

        private void OnStatsChanged()
        {
            Publish<StatsChangedEvent>(new());
        }
    }

    public readonly struct StatChangedEvent : IEntityEvent
    {
        public readonly StatId StatId;

        public StatChangedEvent(StatId statId)
        {
            StatId = statId;
        }
    }

    public readonly struct StatsChangedEvent : IEntityEvent
    {

    }
}
