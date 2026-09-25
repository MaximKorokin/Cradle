using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Quests;
using Assets._Game.Scripts.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public sealed class QuestGiverWindowData : EntityBoundDataAggregatorBase
    {
        private readonly EntityRepository _entityRepository;

        private QuestGiverModule _questGiverModule;
        private QuestModule _questModule;

        public string QuestGiverName { get; set; }
        public IReadOnlyList<QuestDefinition> OfferedQuests { get; private set; } = new QuestDefinition[0];

        public event Action Changed;

        public QuestGiverWindowData(EntityRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }

        protected override void OnBoundEntityChanged(string entityId)
        {
            var giverEntity = _entityRepository.Get(entityId);

            QuestGiverName = giverEntity.Definition.DisplayName;

            _questGiverModule = null;

            if (giverEntity != null && giverEntity.TryGetModule<QuestGiverModule>(out var questGiverModule))
            {
                _questGiverModule = questGiverModule;
            }

            OfferedQuests = _questGiverModule == null ? new QuestDefinition[0] : _questGiverModule.OfferedQuests;
            Changed?.Invoke();
        }

        public void SetTargetEntity(IReadOnlyObservableData<string> targetEntityId)
        {
            var targetEntity = _entityRepository.Get(targetEntityId.Value);
            _questModule = null;

            if (_questModule != null)
            {
                _questModule.Updated -= OnQuestModuleUpdated;
            }

            if (targetEntity != null && targetEntity.TryGetModule<QuestModule>(out var questModule))
            {
                _questModule = questModule;
                _questModule.Updated += OnQuestModuleUpdated;
            }
            Changed?.Invoke();
        }

        public bool IsQuestAccepted(string questId)
        {
            if (_questModule == null) return false;
            return _questModule.AllQuests.Any(q => q.Definition.Id == questId);
        }

        public bool CanCompleteQuest(string questId)
        {
            if (_questModule == null) return false;
            return _questModule.AllQuests.Any(q => q.Definition.Id == questId && q.AreObjectivesCompleted && !q.IsCompleted);
        }

        public QuestState GetQuestState(string questId)
        {
            if (_questModule == null) return null;
            return _questModule.AllQuests.FirstOrDefault(q => q.Definition.Id == questId);
        }

        private void OnQuestModuleUpdated()
        {
            Changed?.Invoke();
        }

        public override void Dispose()
        {
            if (_questModule != null)
                _questModule.Updated -= OnQuestModuleUpdated;
        }
    }
}
