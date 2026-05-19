using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Quests;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public sealed class QuestsHudData : DataAggregatorBase
    {
        private readonly EntityRepository _entityRepository;

        private QuestModule _questModule;

        public IEnumerable<QuestState> ActiveQuests { get; private set; }

        public event Action Changed;

        public QuestsHudData(EntityRepository entityRepository)
        {
            _entityRepository = entityRepository;
            UpdateData();
        }

        public void SetQuestModuleEntity(string questModuleEntityId)
        {
            var entity = _entityRepository.Get(questModuleEntityId);

            if (_questModule != null)
            {
                _questModule.Updated -= OnQuestModuleUpdated;
            }

            if (entity != null && entity.TryGetModule<QuestModule>(out var questsModule))
            {
                _questModule = questsModule;
                _questModule.Updated += OnQuestModuleUpdated;
            }

            UpdateData();
        }

        private void UpdateData()
        {
            if (_questModule == null)
            {
                ActiveQuests = new QuestState[0];
                return;
            }

            ActiveQuests = _questModule.AllQuests.ToArray();

            Changed?.Invoke();
        }

        private void OnQuestModuleUpdated()
        {
            UpdateData();
        }

        public override void Dispose()
        {
            base.Dispose();

            if (_questModule != null)
            {
                _questModule.Updated -= OnQuestModuleUpdated;
            }
        }
    }
}
