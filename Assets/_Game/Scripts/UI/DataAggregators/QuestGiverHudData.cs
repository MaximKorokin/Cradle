using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Quests;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public sealed class QuestGiverHudData : DataAggregatorBase
    {
        private readonly EntityRepository _entityRepository;

        private QuestGiverModule _questGiverModule;
        private QuestModule _questModule;

        public string QuestGiverName { get; set; }
        public IReadOnlyList<QuestDefinition> OfferedQuests { get; private set; } = new QuestDefinition[0];

        public event Action Changed;

        public QuestGiverHudData(EntityRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }

        public void SetEntities(string giverEntityId, string targetEntityId)
        {
            var giverEntity = _entityRepository.Get(giverEntityId);
            var targetEntity = _entityRepository.Get(targetEntityId);

            QuestGiverName = giverEntity.Definition.DisplayName;

            _questGiverModule = null;
            _questModule = null;

            giverEntity?.TryGetModule(out _questGiverModule);

            if (targetEntity != null && targetEntity.TryGetModule<QuestModule>(out var questModule))
            {
                _questModule = questModule;
                _questModule.Updated += OnQuestModuleUpdated;
            }

            UpdateData();
        }

        public bool IsQuestAccepted(string questId)
        {
            if (_questModule == null) return false;
            return _questModule.AllQuests.Any(q => q.Definition.Id == questId);
        }

        private void UpdateData()
        {
            if (_questGiverModule == null)
            {
                OfferedQuests = new QuestDefinition[0];
                Changed?.Invoke();
                return;
            }

            OfferedQuests = _questGiverModule.OfferedQuests;
            Changed?.Invoke();
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
