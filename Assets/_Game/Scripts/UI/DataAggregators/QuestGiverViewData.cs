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
        private Entity _targetEntity;
        private QuestModule _targetQuestModule;

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
            
            if (_targetQuestModule != null)
            {
                _targetQuestModule.Updated -= OnTargetQuestModuleUpdated;
            }
            _targetQuestModule = null;

            if (targetEntity != null && targetEntity.TryGetModule<QuestModule>(out var questModule))
            {
                _targetEntity = targetEntity;
                _targetQuestModule = questModule;
                _targetQuestModule.Updated += OnTargetQuestModuleUpdated;
            }

            NotifyChanged();
        }

        public bool IsQuestAvailable(string questId)
        {
            if (_questGiverModule == null) return false;
            var questDefinition = _questGiverModule.OfferedQuests.FirstOrDefault(q => q.Id == questId);

            if (questDefinition == null ||
                _targetEntity == null ||
                !_targetEntity.TryGetModule<LevelingModule>(out var levelingModule)) return false;
            return questDefinition.RequiredLevel <= levelingModule.Level;
        }

        public bool IsQuestAccepted(string questId)
        {
            if (_targetQuestModule == null) return false;
            return _targetQuestModule.AllQuestSnapshots.Any(q => q.Definition.Id == questId);
        }

        public bool CanCompleteQuest(string questId)
        {
            if (_targetQuestModule == null) return false;
            return _targetQuestModule.AllQuestSnapshots.Any(q => q.Definition.Id == questId && q.AreObjectivesCompleted && !q.IsCompleted);
        }

        public bool IsQuestCompleted(string questId)
        {
            if (_targetQuestModule == null) return false;
            return _targetQuestModule.AllQuestSnapshots.Any(q => q.Definition.Id == questId && q.IsCompleted);
        }

        public QuestStateSnapshot GetQuestStateSnapshot(string questId)
        {
            if (_targetQuestModule == null) return default;
            return _targetQuestModule.GetQuestStateSnapshot(questId);
        }

        private void OnTargetQuestModuleUpdated()
        {
            NotifyChanged();
        }

        public override void Dispose()
        {
            if (_targetQuestModule != null)
                _targetQuestModule.Updated -= OnTargetQuestModuleUpdated;
        }
    }
}
