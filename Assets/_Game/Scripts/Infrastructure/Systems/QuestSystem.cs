using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Querying;
using Assets._Game.Scripts.Quests;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class QuestSystem : EntitySystemBase
    {
        protected override EntityQuery EntityQuery { get; } =
            new EntityQuery(
                RestrictionState.Disabled | RestrictionState.Dead,
                new[] { typeof(QuestModule) }
            );

        public QuestSystem(IGlobalEventBus globalEventBus, EntityRepository repository) : base(globalEventBus, repository)
        {
            TrackEntityEvent<LevelChangedEvent>(OnLevelChanged);
            TrackEntityEvent<InventoryChangedEvent>(OnInventoryChanged);
            TrackGlobalEvent<EntityDiedEvent>(OnEntityDied);

            TrackEntityEvent<QuestUpdatedEvent>(OnQuestUpdated);
            TrackEntityEvent<QuestCompletedEvent>(OnQuestCompleted);
        }

        private void OnLevelChanged(Entity entity, LevelChangedEvent e)
        {
            var questModule = entity.GetModule<QuestModule>();

            HandleEvent(questModule, e);
        }

        private void OnInventoryChanged(Entity entity, InventoryChangedEvent e)
        {
            var questModule = entity.GetModule<QuestModule>();

            HandleEvent(questModule, e);
        }

        private void OnEntityDied(EntityDiedEvent e)
        {
            if (!EntityQuery.Match(e.Killer)) return;
            if (!e.Killer.TryGetModule<QuestModule>(out var questModule)) return;

            HandleEvent(questModule, e);
        }

        private void HandleEvent(QuestModule questModule, IEvent e)
        {
            foreach (var quest in questModule.ActiveQuests)
            {
                foreach (var objective in quest.Objectives)
                {
                    objective.HandleEvent(e);
                }
            }
        }

        private void OnQuestUpdated(Entity entity, QuestUpdatedEvent e)
        {
            var questModule = entity.GetModule<QuestModule>();

            SLog.Info($"Quest updated: {e.QuestState.Title} - {e.QuestState.Objectives[0].CurrentAmount}");
        }

        private void OnQuestCompleted(Entity entity, QuestCompletedEvent e)
        {
            var questModule = entity.GetModule<QuestModule>();

            questModule.RemoveQuest(e.QuestState);

            SLog.Info($"Quest completed: {e.QuestState.Title}");
        }
    }

    public readonly struct QuestUpdatedEvent : IEntityEvent
    {
        public QuestState QuestState { get; }

        public QuestUpdatedEvent(QuestState questState)
        {
            QuestState = questState;
        }
    }

    public readonly struct QuestCompletedEvent : IEntityEvent
    {
        public QuestState QuestState { get; }

        public QuestCompletedEvent(QuestState questState)
        {
            QuestState = questState;
        }
    }
}
