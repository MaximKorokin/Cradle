using Assets._Game.Scripts.Entities.Stats;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Entities.Modules
{
    public class StatsModule : EntityModuleBase
    {
        private readonly StatsController _stats;

        public StatsModule(StatsController stats)
        {
            _stats = stats;
            _stats.StatChanged += OnStatChanged;
            _stats.Changed += OnStatsChanged;
        }

        public IStatsReadOnly Stats => _stats;

        public void AddModifiers(object source, IEnumerable<StatModifier> modifiers)
        {
            _stats.AddModifiers(source, modifiers);
        }

        public void RemoveModifiers(object source)
        {
            _stats.RemoveModifiers(source);
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
