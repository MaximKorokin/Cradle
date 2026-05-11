using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Quests;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public sealed class QuestsHudData : DataAggregatorBase
    {
        private readonly QuestModule _questModule;

        public IEnumerable<QuestState> ActiveQuests { get; private set; }

        public event Action Changed;

        public QuestsHudData(PlayerContext playerContext)
        {
            if (playerContext.Player != null && playerContext.TryGetModule<QuestModule>(out var questsModule))
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
