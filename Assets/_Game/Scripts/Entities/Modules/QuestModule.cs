using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets._Game.Scripts.Infrastructure.Persistence.Codecs;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Quests;
using Assets._Game.Scripts.Quests.Objectives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class QuestModule : EntityModuleBase
    {
        private readonly List<QuestState> _activeQuests = new();
        public IReadOnlyList<QuestState> ActiveQuests => _activeQuests;

        public event Action<QuestState> QuestAdded;
        public event Action<QuestState> QuestUpdated;
        public event Action<QuestState> QuestCompleted;
        public event Action<QuestState> QuestRemoved;

        public void AddQuest(QuestState questState)
        {
            _activeQuests.Add(questState);
            questState.Updated += OnQuestUpdated;
            questState.Completed += OnQuestCompleted;

            QuestAdded?.Invoke(questState);
            // todo: add a list that will publish all events once attached
            //Publish(new QuestAddedEvent(questState));
        }

        public void RemoveQuest(QuestState questState)
        {
            _activeQuests.Remove(questState);
            questState.Updated -= OnQuestUpdated;
            questState.Completed -= OnQuestCompleted;

            QuestRemoved?.Invoke(questState);
        }

        private void OnQuestUpdated(QuestState questState)
        {
            QuestUpdated?.Invoke(questState);
            Entity.Publish(new QuestUpdatedEvent(questState));
        }

        private void OnQuestCompleted(QuestState questState)
        {
            QuestCompleted?.Invoke(questState);
            Entity.Publish(new QuestCompletedEvent(questState));
        }
    }

    public sealed class QuestModuleFactory : IEntityModuleFactory, IEntityModulePersistance
    {
        private readonly CodecRegistry _codecRegistry;

        public QuestModuleFactory(CodecRegistry codecRegistry)
        {
            _codecRegistry = codecRegistry;
        }

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

        public void Apply(Entity entity, EntitySave entitySave)
        {
            if (!entity.TryGetModule<QuestModule>(out var questModule) || entitySave.QuestSaves == null) return;

            for (int i = 0; i < entitySave.QuestSaves.Length; i++)
            {
                var questSave = entitySave.QuestSaves[i];
                var questState = questModule.ActiveQuests.FirstOrDefault(x => x.Definition.Id == questSave.DefinitionId);
                if (questState == null) continue;

                for (int j = 0; j < questSave.ProgressSaves.Length; j++)
                {
                    var progressSave = questSave.ProgressSaves[j];
                    if (progressSave == null) continue;
                    var data = _codecRegistry.DecodeOrNull(progressSave);
                    if (data == null) continue;
                    foreach (var objective in questState.Objectives.OfType<ISaveableObjectiveProgress>())
                        if (objective.TryLoad(data))
                            break;
                }
            }
        }

        public void Save(Entity entity, EntitySave entitySave)
        {
            if (!entity.TryGetModule<QuestModule>(out var questModule)) return;

            entitySave.QuestSaves = new QuestStateSave[questModule.ActiveQuests.Count];
            for (int i = 0; i < questModule.ActiveQuests.Count; i++)
            {
                var questState = questModule.ActiveQuests[i];
                entitySave.QuestSaves[i] = new QuestStateSave
                {
                    DefinitionId = questState.Definition.Id,
                    ProgressSaves = questState.Objectives
                        .OfType<ISaveableObjectiveProgress>()
                        .Select(x => _codecRegistry.EncodeOrNull(x.Save()))
                        .Where(x => x != null)
                        .ToArray()
                };
            }
        }
    }
}
