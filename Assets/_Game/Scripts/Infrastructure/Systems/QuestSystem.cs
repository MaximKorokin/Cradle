using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Querying;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Quests;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class QuestSystem : EntitySystemBase
    {
        private readonly ItemStackFactory _itemStackFactory;

        protected override EntityQuery EntityQuery { get; } =
            new EntityQuery(
                RestrictionState.Disabled | RestrictionState.Dead,
                new[] { typeof(QuestModule) }
            );

        public QuestSystem(
            IGlobalEventBus globalEventBus,
            EntityRepository repository,
            ItemStackFactory itemStackFactory) : base(globalEventBus, repository)
        {
            _itemStackFactory = itemStackFactory;

            TrackEntityEvent<LevelChangedEvent>(OnLevelChanged);
            TrackEntityEvent<InventoryChangedEvent>(OnInventoryChanged);
            TrackGlobalEvent<EntityDiedEvent>(OnEntityDied);

            TrackEntityEvent<QuestAddedEvent>(OnQuestAdded);
            TrackEntityEvent<QuestUpdatedEvent>(OnQuestUpdated);
            TrackEntityEvent<QuestObjectivesCompletedEvent>(OnQuestObjectivesCompleted);
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
            for (int i = 0; i < questModule.AllQuests.Count; i++)
            {
                var quest = questModule.AllQuests[i];

                if (quest.IsCompleted) continue;

                // Handle the event for each objective in the quest
                for (int j = 0; j < quest.Objectives.Length; j++)
                {
                    var objective = quest.Objectives[j];
                    objective.HandleEvent(e);
                }
            }
        }

        private void OnQuestAdded(Entity entity, QuestAddedEvent e)
        {
            SLog.Info($"Quest added: {e.QuestState.Title}");

            InitializeQuest(e.QuestState, entity);
        }

        private void OnQuestUpdated(Entity entity, QuestUpdatedEvent e)
        {
            SLog.Info($"Quest updated: {e.QuestState.Title} - {e.QuestState.Objectives[0].CurrentAmount}");
        }

        private void OnQuestObjectivesCompleted(Entity entity, QuestObjectivesCompletedEvent e)
        {
            SLog.Info($"Quest objectives completed: {e.QuestState.Title}");

            e.QuestState.SetCompleted(true);
        }

        private void OnQuestCompleted(Entity entity, QuestCompletedEvent e)
        {
            SLog.Info($"Quest completed: {e.QuestState.Title}");

            var reward = e.QuestState.Definition.Reward;
            if (reward == null) return;

            if (reward.Experience > 0 && entity.TryGetModule<LevelingModule>(out var levelingModule))
            {
                levelingModule.AddExperience(reward.Experience);
            }

            if (reward.ItemRewards != null && reward.ItemRewards.Length > 0)
            {
                entity.TryGetModule<InventoryModule>(out var inventoryModule);
                entity.TryGetModule<SpatialModule>(out var spatialModule);

                foreach (var itemDefinition in reward.ItemRewards)
                {
                    var itemStack = _itemStackFactory.Create(itemDefinition.Id, 1);
                    var snapshot = new ItemStackSnapshot(itemStack.Definition, itemStack.InstanceData, itemStack.Amount);

                    var remaining = snapshot.Amount;
                    if (inventoryModule != null)
                        remaining -= inventoryModule.Inventory.Add(snapshot);

                    if (remaining > 0 && spatialModule != null)
                        GlobalEventBus.Publish(new LootItemDropRequestedEvent(spatialModule.Position, itemDefinition, remaining));
                }
            }
        }

        private void InitializeQuest(QuestState questState, Entity entity)
        {
            foreach (var objective in questState.Objectives)
            {
                objective.Initialize(entity);
            }
        }
    }

    public readonly struct QuestAddedEvent : IEntityEvent
    {
        public QuestState QuestState { get; }

        public QuestAddedEvent(QuestState questState)
        {
            QuestState = questState;
        }
    }

    public readonly struct QuestRemovedEvent : IEntityEvent
    {
        public QuestState QuestState { get; }

        public QuestRemovedEvent(QuestState questState)
        {
            QuestState = questState;
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

    public readonly struct QuestObjectivesCompletedEvent : IEntityEvent
    {
        public QuestState QuestState { get; }

        public QuestObjectivesCompletedEvent(QuestState questState)
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
