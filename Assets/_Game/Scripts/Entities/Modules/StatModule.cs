using Assets._Game.Scripts.Entities.Stats;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Entities.Modules
{
    public class StatModule : EntityModuleBase
    {
        private readonly StatsController _statsController;

        public StatModule(StatsController stats)
        {
            _statsController = stats;

            _statsController.StatChanged += OnStatChanged;
            _statsController.Changed += OnStatsChanged;
        }

        public StatsController Stats => _statsController;

        public void SetBase(StatId statId, float value)
        {
            _statsController.SetBase(statId, value);
        }

        public void AddModifiers(StatModifierSource source, IEnumerable<StatModifier> modifiers)
        {
            _statsController.AddModifiers(source, modifiers);
        }

        public void RemoveModifiers(StatModifierSource source)
        {
            _statsController.RemoveModifiers(source);
        }

        private void OnStatChanged(StatId id)
        {
            Publish<StatChangedEvent>(new(Entity, id));
        }

        private void OnStatsChanged()
        {
            Publish<StatsChangedEvent>(new(Entity));
        }
    }

    public readonly struct StatChangedEvent : IEntityEvent
    {
        public readonly StatId StatId;

        public Entity Entity { get; }

        public StatChangedEvent(Entity entity, StatId statId)
        {
            Entity = entity;
            StatId = statId;
        }
    }

    public readonly struct StatsChangedEvent : IEntityEvent
    {
        public Entity Entity { get; }
        public StatsChangedEvent(Entity entity)
        {
            Entity = entity;
        }
    }
}
