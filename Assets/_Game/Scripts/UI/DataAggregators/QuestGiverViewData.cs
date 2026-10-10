using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Quests;
using Assets._Game.Scripts.Shared;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public sealed class QuestGiverViewData : EntityBoundDataAggregatorBase
    {
        private QuestGiverModule _questGiverModule;
        private QuestModule _questModule;

        public string QuestGiverName { get; set; }
        public IReadOnlyObservableData<EntryRef> TargetEntityId { get; private set; }
        public IReadOnlyList<QuestDefinition> OfferedQuests { get; private set; } = new QuestDefinition[0];

        public QuestGiverViewData(EntityRepository entityRepository) : base(entityRepository) { }

        protected override void OnBoundEntityChanged()
        {
            var giverEntity = Entity;
            QuestGiverName = giverEntity.Definition.DisplayName;

            _questGiverModule = null;

            if (giverEntity != null && giverEntity.TryGetModule<QuestGiverModule>(out var questGiverModule))
            {
                _questGiverModule = questGiverModule;
            }

            OfferedQuests = _questGiverModule == null ? new QuestDefinition[0] : _questGiverModule.OfferedQuests;

            NotifyChanged();
        }

        public void SetTargetEntity(IReadOnlyObservableData<EntryRef> targetEntityId)
        {
            var targetEntryRef = targetEntityId?.Value ?? default;
            TargetEntityId = targetEntityId;
            EntityRepository.TryGet(targetEntryRef, out var targetEntity);
            
            if (_questModule != null)
            {
                _questModule.Updated -= OnTargetQuestModuleUpdated;
            }
            _questModule = null;

            if (targetEntity != null && targetEntity.TryGetModule<QuestModule>(out var questModule))
            {
                _questModule = questModule;
                _questModule.Updated += OnTargetQuestModuleUpdated;
            }

            NotifyChanged();
        }

        public bool IsQuestAccepted(string questId)
        {
            if (_questModule == null) return false;
            return _questModule.AllQuestSnapshots.Any(q => q.Definition.Id == questId);
        }

        public bool CanCompleteQuest(string questId)
        {
            if (_questModule == null) return false;
            return _questModule.AllQuestSnapshots.Any(q => q.Definition.Id == questId && q.AreObjectivesCompleted && !q.IsCompleted);
        }

        public QuestStateSnapshot GetQuestStateSnapshot(string questId)
        {
            if (_questModule == null) return default;
            return _questModule.GetQuestStateSnapshot(questId);
        }

        private void OnTargetQuestModuleUpdated()
        {
            NotifyChanged();
        }

        public override void Dispose()
        {
            if (_questModule != null)
                _questModule.Updated -= OnTargetQuestModuleUpdated;
        }
    }
}
