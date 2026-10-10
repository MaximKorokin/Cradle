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
    public sealed class QuestModule : EntityModuleBase, IResettableModule
    {
        private readonly List<QuestState> _allQuestStates = new();
        public IReadOnlyList<QuestState> AllQuestStates => _allQuestStates.ToList();
        public IReadOnlyList<QuestStateSnapshot> AllQuestSnapshots => _allQuestStates.Select(q => q.Snapshot).ToList();

        public event Action Updated;
        public event Action<QuestStateSnapshot> QuestAdded;
        public event Action<QuestStateSnapshot> QuestUpdated;
        public event Action<QuestStateSnapshot> QuestObjectivesCompleted;
        public event Action<QuestStateSnapshot> QuestCompleted;
        public event Action<QuestStateSnapshot> QuestRemoved;

        public QuestState GetQuestState(string questDefinitionId)
        {
            return _allQuestStates.FirstOrDefault(q => q.Definition.Id == questDefinitionId);
        }

        public QuestStateSnapshot GetQuestStateSnapshot(string questDefinitionId)
        {
            var questState = _allQuestStates.FirstOrDefault(q => q.Definition.Id == questDefinitionId);
            return questState == null ? default : questState.Snapshot;
        }

        public bool HasQuest(string questDefinitionId)
        {
            return _allQuestStates.Any(q => q.Definition.Id == questDefinitionId);
        }

        public void AddQuest(QuestDefinition questDefinition)
        {
            if (HasQuest(questDefinition.Id)) return;

            var questState = new QuestState(questDefinition);
            AddQuestState(questState);
        }

        public void AddQuestState(QuestState questState)
        {
            _allQuestStates.Add(questState);
            questState.Updated += OnQuestUpdated;
            questState.ObjectivesCompleted += OnQuestObjectivesCompleted;
            questState.Completed += OnQuestCompleted;

            var snapshot = questState.Snapshot;
            QuestAdded?.Invoke(snapshot);
            Updated?.Invoke();
            Publish(new QuestAddedEvent(snapshot));
        }

        public void RemoveQuest(QuestState questState)
        {
            if (!HasQuest(questState.Definition.Id)) return;

            _allQuestStates.Remove(questState);
            questState.Updated -= OnQuestUpdated;
            questState.ObjectivesCompleted -= OnQuestObjectivesCompleted;
            questState.Completed -= OnQuestCompleted;

            var snapshot = questState.Snapshot;
            QuestRemoved?.Invoke(snapshot);
            Updated?.Invoke();
            Publish(new QuestRemovedEvent(snapshot));
        }

        private void OnQuestUpdated(QuestState questState)
        {
            var snapshot = questState.Snapshot;
            QuestUpdated?.Invoke(snapshot);
            Updated?.Invoke();
            Publish(new QuestUpdatedEvent(snapshot));
        }

        private void OnQuestObjectivesCompleted(QuestState questState)
        {
            var snapshot = questState.Snapshot;
            QuestObjectivesCompleted?.Invoke(snapshot);
            Updated?.Invoke();
            Publish(new QuestObjectivesCompletedEvent(snapshot));
        }

        private void OnQuestCompleted(QuestState questState)
        {
            var snapshot = questState.Snapshot;
            QuestCompleted?.Invoke(snapshot);
            Updated?.Invoke();
            Publish(new QuestCompletedEvent(snapshot));
        }

        public void Reset()
        {
            foreach (var quests in _allQuestStates.ToArray())
            {
                RemoveQuest(quests);
            }
        }
    }

    public sealed class QuestModuleFactory : IEntityModuleFactory, IEntityModulePersistance
    {
        private readonly CodecRegistry _codecRegistry;
        private readonly QuestDefinitionCatalog _questDefinitionCatalog;

        public QuestModuleFactory(CodecRegistry codecRegistry, QuestDefinitionCatalog questDefinitionCatalog)
        {
            _codecRegistry = codecRegistry;
            _questDefinitionCatalog = questDefinitionCatalog;
        }

        public EntityModuleBase Create(EntityDefinition entityDefinition)
        {
            if (!entityDefinition.TryGetModuleDefinition<QuestModuleDefinition>(out var questModuleDefinition))
            {
                return null;
            }

            var questModule = new QuestModule();

            return questModule;
        }

        public void Apply(Entity entity, EntitySave entitySave)
        {
            if (!entity.TryGetModule<QuestModule>(out var questModule) || entitySave.QuestSaves == null) return;

            for (int i = 0; i < entitySave.QuestSaves.Length; i++)
            {
                var questSave = entitySave.QuestSaves[i];

                if (questModule.HasQuest(questSave.DefinitionId) ||
                    !_questDefinitionCatalog.TryGet(questSave.DefinitionId, out var questDefinition)) continue;

                var questState = new QuestState(questDefinition);

                // If quest is completed, we should not raise Completed event
                // If quest was completed but now is not, we should not allow to complete it again
                questState.SetCompleted(questSave.IsCompleted, true);

                for (int j = 0; j < questSave.ProgressSaves.Length; j++)
                {
                    var progressSave = questSave.ProgressSaves[j];
                    if (progressSave == null) continue;
                    var data = _codecRegistry.DecodeOrNull(progressSave);
                    if (data == null) continue;
                    foreach (var objective in questState.Objectives.OfType<ILoadableObjectiveProgress>())
                        if (objective.TryLoad(data))
                            break;
                }

                questModule.AddQuestState(questState);
            }
        }

        public void Save(Entity entity, EntitySave entitySave)
        {
            if (!entity.TryGetModule<QuestModule>(out var questModule)) return;

            var quests = questModule.AllQuestStates;
            entitySave.QuestSaves = new QuestStateSave[quests.Count];
            for (int i = 0; i < quests.Count; i++)
            {
                var questState = quests[i];
                entitySave.QuestSaves[i] = new QuestStateSave
                {
                    DefinitionId = questState.Definition.Id,
                    IsCompleted = questState.IsCompleted,
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
