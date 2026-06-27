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
        private SelectableTabsController _questsTabsController;
        [SerializeField]
        private SimpleListView _questsListViewTemplate;

        private SimpleListView _activeQuestsListView;
        private SimpleListView _completedQuestsListView;

        public event Action<string> QuestInfoClicked;
        public event Action<string> QuestActionClicked;

        public void Render(QuestsHudData data)
        {
            // Active quests
            _activeQuestsListView = Instantiate(_questsListViewTemplate);
            _activeQuestsListView.Render(data.ActiveQuests.Where(q => !q.IsCompleted).Select(q => new SimpleListItemData()
            {
                Identifier = q.Definition.Id,
                Sprite = null,
                Text = q.Definition.Title,
            }));
            _questsTabsController.AddTab(new("Active", _activeQuestsListView.transform as RectTransform));
            _activeQuestsListView.ElementInfoClicked += OnQuestInfoClicked;
            _activeQuestsListView.ElementActionClicked += OnQuestActionClicked;

            // Completed quests
            _completedQuestsListView = Instantiate(_questsListViewTemplate);
            _completedQuestsListView.Render(data.ActiveQuests.Where(q => q.IsCompleted).Select(q => new SimpleListItemData()
            {
                Identifier = q.Definition.Id,
                Sprite = null,
                Text = q.Definition.Title,
            }));
            _questsTabsController.AddTab(new("Completed", _completedQuestsListView.transform as RectTransform));
            _completedQuestsListView.ElementInfoClicked += OnQuestInfoClicked;
            _completedQuestsListView.ElementActionClicked += OnQuestActionClicked;
        }

        public override void OnShow()
        {
            base.OnShow();

            _questsListViewTemplate.gameObject.SetActive(false);
        }

        public override void OnHide()
        {
            base.OnHide();

            _activeQuestsListView.ElementInfoClicked -= OnQuestInfoClicked;
            _activeQuestsListView.ElementActionClicked -= OnQuestActionClicked;
            _completedQuestsListView.ElementInfoClicked -= OnQuestInfoClicked;
            _completedQuestsListView.ElementActionClicked -= OnQuestActionClicked;

            _activeQuestsListView.Clear();
            _completedQuestsListView.Clear();

            _questsTabsController.ClearTabs();
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
