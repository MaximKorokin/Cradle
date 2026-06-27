using Assets._Game.Scripts.UI.Common;
using Assets._Game.Scripts.UI.DataAggregators;
using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class QuestGiverWindow : UIWindowBase
    {
        [SerializeField]
        private TMP_Text _questGiverName;
        [SerializeField]
        private SimpleListView _availableQuestsListView;
        [SerializeField]
        private SimpleListView _canCompleteQuestsListView;

        public event Action<string> QuestInfoClicked;
        public event Action<string> QuestAcceptClicked;
        public event Action<string> QuestCompleteClicked;

        public void Render(QuestGiverHudData data)
        {
            _questGiverName.text = data.QuestGiverName;

            _canCompleteQuestsListView.Render(data.OfferedQuests.Where(q => data.IsQuestAccepted(q.Id) && data.CanCompleteQuest(q.Id)).Select(q => new SimpleListItemData()
            {
                Identifier = q.Id,
                Sprite = null,
                Text = q.Title
            }));

            _canCompleteQuestsListView.ElementInfoClicked += OnQuestInfoClicked;
            _canCompleteQuestsListView.ElementActionClicked += OnQuestCompleteClicked;

            _availableQuestsListView.Render(data.OfferedQuests.Where(q => !data.IsQuestAccepted(q.Id)).Select(q => new SimpleListItemData()
            {
                Identifier = q.Id,
                Sprite = null,
                Text = q.Title
            }));

            _availableQuestsListView.ElementInfoClicked += OnQuestInfoClicked;
            _availableQuestsListView.ElementActionClicked += OnQuestAcceptClicked;
        }

        public override void OnHide()
        {
            base.OnHide();

            _canCompleteQuestsListView.ElementInfoClicked -= OnQuestInfoClicked;
            _canCompleteQuestsListView.ElementActionClicked -= OnQuestCompleteClicked;

            _availableQuestsListView.ElementInfoClicked -= OnQuestInfoClicked;
            _availableQuestsListView.ElementActionClicked -= OnQuestAcceptClicked;
        }

        private void OnQuestInfoClicked(string questId)
        {
            QuestInfoClicked?.Invoke(questId);
        }

        private void OnQuestAcceptClicked(string questId)
        {
            QuestAcceptClicked?.Invoke(questId);
        }

        private void OnQuestCompleteClicked(string questId)
        {
            QuestCompleteClicked?.Invoke(questId);
        }
    }
}
