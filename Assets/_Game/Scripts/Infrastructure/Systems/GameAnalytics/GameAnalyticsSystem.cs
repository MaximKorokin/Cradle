using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Interactions.Steps;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Configs;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Persistence.DataTypes;
using Assets._Game.Scripts.Infrastructure.Querying;
using Assets._Game.Scripts.Locations;

namespace Assets._Game.Scripts.Infrastructure.Systems.GameAnalytics
{
    public sealed class GameAnalyticsSystem : EntitySystemBase
    {
        private readonly GameAnalyticsConfig _gameAnalyticsConfig;
        private readonly GameStatisticsService _statisticsService;
        private readonly GameLoggingService _loggingService;
        private readonly ILocationContext _locationContext;

        protected override EntityQuery EntityQuery { get; } = new EntityQuery();

        public GameAnalyticsSystem(
            IGlobalEventBus globalEventBus,
            EntityRepository entityRepository,
            GameAnalyticsConfig gameAnalyticsConfig,
            GameStatisticsService statisticsService,
            GameLoggingService loggingService,
            ILocationContext locationContext) : base(globalEventBus, entityRepository)
        {
            _gameAnalyticsConfig = gameAnalyticsConfig;
            _statisticsService = statisticsService;
            _loggingService = loggingService;
            _locationContext = locationContext;

            TrackGlobalEvent<GameAnalyticsEvent>(OnAction);
            TrackGlobalEvent<SaveGameRequest>(_ =>
            {
                _statisticsService.SaveStatistics();
                _loggingService.SaveLogs();
            });

            TrackGlobalEvent<DamageAppliedEvent>(e => Publish(e.IsCritical ? AnalyticsEntryType.CriticalDamageApply : AnalyticsEntryType.DamageApply, e.Source, e.Target, e.Damage, e.SourceType.ToString()));
            TrackGlobalEvent<HealAppliedEvent>(e => Publish(AnalyticsEntryType.HealApply, e.Source, e.Target, e.Heal));
            TrackGlobalEvent<AddExperienceRequestEvent>(e => Publish(AnalyticsEntryType.ExperienceGain, e.Source, e.Target, e.Experience));
            TrackGlobalEvent<EntityDiedEvent>(e => Publish(AnalyticsEntryType.EntityDeath, e.Killer, e.Victim));
            TrackGlobalEvent<EntityRevivedEvent>(e => Publish(AnalyticsEntryType.EntityRevive, e.Source, e.Entity));
            TrackGlobalEvent<EntitySpawnedEvent>(e => Publish(AnalyticsEntryType.EntitySpawn, e.Entity, null, 1));
            TrackGlobalEvent<EntityDespawningEvent>(e => Publish(AnalyticsEntryType.EntityDespawn, e.Entity, null, 1));
            TrackGlobalEvent<LocationChangedEvent>(e => Publish(AnalyticsEntryType.LocationChange, null, null, 1, _locationContext.CurrentLocation.DisplayName));
            
            TrackEntityEvent<EntityRepositionRequest>((entity, e) => Publish(AnalyticsEntryType.EntityReposition, entity, null, 1, e.Position.ToString()));
            TrackEntityEvent<QuestAddedEvent>((entity, e) => Publish(AnalyticsEntryType.QuestAdd, entity, null, 1, e.QuestState.Definition.Title));
            TrackEntityEvent<QuestCompletedEvent>((entity, e) => Publish(AnalyticsEntryType.QuestComplete, entity, null, 1, e.QuestState.Definition.Title));
        }

        private void Publish(AnalyticsEntryType type, Entity source, Entity target, float value = 0f, string details = null)
        {
            GlobalEventBus.Publish(new GameAnalyticsEvent(type, source, target, value, details));
        }

        private void OnAction(GameAnalyticsEvent e)
        {
            var source = GetEntityName(e.Source);
            var target = GetEntityName(e.Target);
            var location = _locationContext.CurrentLocation;
            var place = location == null ? "-" : !string.IsNullOrEmpty(location.DisplayName) ? location.DisplayName : location.SceneName;
            var position = GetEntityPosition(e.Target ?? e.Source);

            _statisticsService.AddEntry(new(e.Type, place, source, target, e.Value));
            _loggingService.AddEntry(new(e.Type, place, position, source, target, e.Value, e.Details));
        }

        private static string GetEntityName(Entity entity)
        {
            if (entity == null) return "-";
            var name = entity.Definition != null ? entity.Definition.DisplayName : null;
            return string.IsNullOrEmpty(name) ? entity.Id : name;
        }

        private static SerializableVector2 GetEntityPosition(Entity entity)
        {
            if (entity != null && entity.TryGetModule(out SpatialModule spatial))
                return new(spatial.Position.x, spatial.Position.y);
            return new(float.NaN, float.NaN);
        }
    }

    public readonly struct GameAnalyticsEvent : IGlobalEvent
    {
        public readonly AnalyticsEntryType Type;
        public readonly Entity Source;
        public readonly Entity Target;
        public readonly float Value;
        public readonly string Details;

        public GameAnalyticsEvent(AnalyticsEntryType type, Entity source = null, Entity target = null, float value = 0f, string details = null)
        {
            Type = type;
            Source = source;
            Target = target;
            Value = value;
            Details = details;
        }
    }
}
