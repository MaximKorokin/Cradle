using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Quests;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class QuestModule : EntityModuleBase
    {
        private readonly List<QuestState> _activeQuests = new();
        public IReadOnlyList<QuestState> ActiveQuests => _activeQuests;

        public void AddQuest(QuestState questState)
        {
            _activeQuests.Add(questState);
            questState.Updated += OnQuestUpdated;
        }

        public void RemoveQuest(QuestState questState)
        {
            _activeQuests.Remove(questState);
            questState.Updated -= OnQuestUpdated;
        }

        private void OnQuestUpdated(QuestState questState)
        {
            Entity.Publish(new QuestUpdatedEvent(questState));
        }

        private void OnQuestCompleted(QuestState questState)
        {
            Entity.Publish(new QuestCompletedEvent(questState));
        }
    }

    public sealed class QuestModuleFactory : IEntityModuleFactory
    {
        public EntityModuleBase Create(EntityDefinition entityDefinition)
        {
            if (!entityDefinition.TryGetModuleDefinition<QuestModuleDefinition>(out var questModuleDefinition))
            {
                return null;
            }

            var questModule = new QuestModule();
            foreach (var questDefinition in questModuleDefinition.InitialQuests)
            {
                var questState = new QuestState(questDefinition);
                questModule.AddQuest(questState);
            }

            return questModule;
        }
    }
}
