using Assets._Game.Scripts.UI.Common;
using Assets._Game.Scripts.UI.DataAggregators;
using System;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class QuestsView : UIViewBase<QuestsViewData>
    {
        [SerializeField]
        private SelectableTabsController _questsTabsController;
        [SerializeField]
        private SimpleListView _questsListViewTemplate;

        private SimpleListView _activeQuestsListView;
        private SimpleListView _completedQuestsListView;

        public event Action<string> QuestInfoClicked;
        public event Action<string> QuestActionClicked;

        protected override void Awake()
        {
            base.Awake();
            _questsListViewTemplate.gameObject.SetActive(false);
        }

        protected override void Render(QuestsViewData data)
        {
            Clear();

            var currentTabIndex = _questsTabsController.GetSelectedTabIndex();

            // Clear previous tabs
            _questsTabsController.ClearTabs();

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

            _questsTabsController.SelectTab(currentTabIndex);
        }

        private void Clear()
        {
            if (_activeQuestsListView != null)
            {
                _activeQuestsListView.ElementInfoClicked -= OnQuestInfoClicked;
                _activeQuestsListView.ElementActionClicked -= OnQuestActionClicked;
                _activeQuestsListView.Clear();
            }

            if (_completedQuestsListView != null)
            {
                _completedQuestsListView.ElementInfoClicked -= OnQuestInfoClicked;
                _completedQuestsListView.ElementActionClicked -= OnQuestActionClicked;
                _completedQuestsListView.Clear();
            }

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
