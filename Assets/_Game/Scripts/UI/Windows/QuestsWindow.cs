using Assets._Game.Scripts.UI.Common;
using Assets._Game.Scripts.UI.DataAggregators;
using System;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class QuestsWindow : UIWindowBase
    {
        [SerializeField]
        private SimpleListView _questsListView;

        public event Action<string> QuestInfoClicked;
        public event Action<string> QuestActionClicked;

        public void Render(QuestsHudData data)
        {
            _questsListView.Render(data.ActiveQuests.Select(q => new SimpleListItemData()
            {
                Identifier = q.Definition.Id,
                Sprite = null,
                Text = q.Title
            }));

            _questsListView.ElementInfoClicked -= OnQuestInfoClicked;
            _questsListView.ElementInfoClicked += OnQuestInfoClicked;
            _questsListView.ElementActionClicked -= OnQuestActionClicked;
            _questsListView.ElementActionClicked += OnQuestActionClicked;
        }

        public override void OnHide()
        {
            base.OnHide();

            _questsListView.ElementInfoClicked -= OnQuestInfoClicked;
            _questsListView.ElementActionClicked -= OnQuestActionClicked;
        }

        private void OnQuestInfoClicked(string questId)
        {
            QuestInfoClicked?.Invoke(questId);
        }

        private void OnQuestActionClicked(string questId)
        {
            QuestActionClicked?.Invoke(questId);
        }
    }
}
