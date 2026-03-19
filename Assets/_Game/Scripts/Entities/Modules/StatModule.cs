using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Infrastructure.Persistence;
using System.Collections.Generic;
using System.Linq;

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

    public sealed class StatModuleFactory : IEntityModuleFactory, IEntityModulePersistance<StatModule, StatsSave>
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

        public void Apply(StatModule statsModule, StatsSave statsSave)
        {
            _statsControllerAssembler.Apply(statsModule.Stats as StatsController, statsSave);
        }

        public StatsSave Save(StatModule module)
        {
            return _statsControllerAssembler.Save(module.Stats);
        }
    }
}
