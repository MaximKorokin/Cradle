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
        private SimpleListView _questsListView;

        public event Action<string> QuestInfoClicked;
        public event Action<string> QuestAcceptClicked;

        public void Render(QuestGiverHudData data)
        {
            _questGiverName.text = data.QuestGiverName;
            _questsListView.Render(data.OfferedQuests.Select(q => new SimpleListItemData()
            {
                Identifier = q.Id,
                Sprite = null,
                Text = q.Title
            }));

            _questsListView.ElementInfoClicked -= OnQuestInfoClicked;
            _questsListView.ElementInfoClicked += OnQuestInfoClicked;
            _questsListView.ElementActionClicked -= OnQuestAcceptClicked;
            _questsListView.ElementActionClicked += OnQuestAcceptClicked;
        }

        public override void OnHide()
        {
            base.OnHide();

            _questsListView.ElementInfoClicked -= OnQuestInfoClicked;
            _questsListView.ElementActionClicked -= OnQuestAcceptClicked;
        }

        private void OnQuestInfoClicked(string questId)
        {
            QuestInfoClicked?.Invoke(questId);
        }

        private void OnQuestAcceptClicked(string questId)
        {
            QuestAcceptClicked?.Invoke(questId);
        }
    }
}
