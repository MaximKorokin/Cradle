using Assets._Game.Scripts.Entities.Stats;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Entities.Modules
{
    public class StatModule : EntityModuleBase
    {
        private readonly StatsController _statsController;

        public StatsController Stats => _statsController;

        public StatModule(StatsController stats)
        {
            _statsController = stats;

            _statsController.StatChanged += OnStatChanged;
            _statsController.Changed += OnStatsChanged;
        }

        public void SetBase(StatId statId, float value)
        {
            _statsController.SetBase(statId, value);
        }

        public void AddBase(StatId statId, float value)
        {
            var currentValue = _statsController.GetBase(statId);
            _statsController.SetBase(statId, currentValue + value);
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
            Publish(new StatChangedEvent(id));
        }

        private void OnStatsChanged()
        {
            Publish(new StatsChangedEvent());
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

    public sealed class StatModuleFactory : IEntityModuleFactory
    {
        private readonly StatsControllerAssembler _statsControllerAssembler;

        public StatModuleFactory(StatsControllerAssembler statsControllerAssembler)
        {
            _statsControllerAssembler = statsControllerAssembler;
        }

        public EntityModuleBase Create(EntityDefinition entityDefinition)
        {
            if (entityDefinition.TryGetModuleDefinition<StatsModuleDefinition>(out var statsDefinitionModule))
            {
                var statsController = _statsControllerAssembler.Create(statsDefinitionModule.Stats);
                return new StatModule(statsController);
            }
            return null;
        }
    }
}
