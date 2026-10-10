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

        private TabbedListViewCollection _tabbedListViews;

        public event Action<string> QuestInfoClicked;
        public event Action<string> QuestActionClicked;

        protected override void Awake()
        {
            base.Awake();
            _questsListViewTemplate.gameObject.SetActive(false);
            _tabbedListViews = new TabbedListViewCollection(_questsTabsController, _questsListViewTemplate);
        }

        protected override void Render(QuestsViewData data)
        {
            var currentTabIndex = _questsTabsController.GetSelectedTabIndex();
            Clear();

            // Active quests
            _tabbedListViews.AddTab("Active", "Active", data.AllQuests.Where(q => !q.IsCompleted).Select(q => new SimpleListItemData()
            {
                Identifier = q.Definition.Id,
                Sprite = null,
                Text = q.Definition.Title,
            }), OnQuestInfoClicked, OnQuestActionClicked);

            // Completed quests
            _tabbedListViews.AddTab("Completed", "Completed", data.AllQuests.Where(q => q.IsCompleted).Select(q => new SimpleListItemData()
            {
                Identifier = q.Definition.Id,
                Sprite = null,
                Text = q.Definition.Title,
            }), OnQuestInfoClicked, OnQuestActionClicked);

            _questsTabsController.SelectTab(currentTabIndex);
        }

        private void Clear()
        {
            _tabbedListViews.Clear();
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
