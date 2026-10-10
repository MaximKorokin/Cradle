using Assets._Game.Scripts.Quests;
using Assets._Game.Scripts.UI.Common;
using Assets._Game.Scripts.UI.DataAggregators;
using NUnit.Framework;
using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class QuestGiverView : UIViewBase<QuestGiverViewData>
    {
        [SerializeField]
        private TMP_Text _questGiverName;
        [SerializeField]
        private SelectableTabsController _questsTabsController;
        [SerializeField]
        private SimpleListView _questListViewTemplate;
        [SerializeField]
        private SimpleListItemView _canTakeQuestListItemViewTemplate;
        [SerializeField]
        private SimpleListItemView _inProgressQuestListItemViewTemplate;
        [SerializeField]
        private SimpleListItemView _canCompleteQuestListItemViewTemplate;
        [SerializeField]
        private SimpleListItemView _unavailableQuestListItemViewTemplate;
        [SerializeField]
        private SimpleListItemView _completedQuestListItemViewTemplate;

        private TabbedListViewCollection _tabbedListViews;

        public event Action<string> QuestInfoClicked;
        public event Action<string> QuestAcceptClicked;
        public event Action<string> QuestCompleteClicked;

        protected override void Awake()
        {
            base.Awake();

            _questListViewTemplate.gameObject.SetActive(false);

            _canTakeQuestListItemViewTemplate.gameObject.SetActive(false);
            _inProgressQuestListItemViewTemplate.gameObject.SetActive(false);
            _canCompleteQuestListItemViewTemplate.gameObject.SetActive(false);
            _unavailableQuestListItemViewTemplate.gameObject.SetActive(false);
            _completedQuestListItemViewTemplate.gameObject.SetActive(false);

            _tabbedListViews = new TabbedListViewCollection(_questsTabsController, _questListViewTemplate);
        }

        protected override void Render(QuestGiverViewData data)
        {
            _questGiverName.text = data.QuestGiverName;
            var currentTabIndex = _questsTabsController.GetSelectedTabIndex();
            _tabbedListViews.Clear();

            var unavalableQuests = data.OfferedQuests.Where(q => !data.IsQuestAccepted(q.Id) && !data.IsQuestAvailable(q.Id)).ToArray();
            var completedQuests = data.OfferedQuests.Where(q => data.IsQuestAccepted(q.Id) && data.IsQuestCompleted(q.Id)).ToArray();
            var availableQuests = data.OfferedQuests.Except(unavalableQuests).Except(completedQuests).ToArray();
            var canTakeQuests = availableQuests.Where(q => !data.IsQuestAccepted(q.Id)).ToArray();
            var inProgressQuests = availableQuests.Where(q => data.IsQuestAccepted(q.Id) && !data.IsQuestCompleted(q.Id) && !data.CanCompleteQuest(q.Id)).ToArray();
            var canCompleteQuests = availableQuests.Where(q => data.IsQuestAccepted(q.Id) && data.CanCompleteQuest(q.Id)).ToArray();

            _tabbedListViews.AddTab("Available", "Available", canCompleteQuests.Concat(canTakeQuests).Concat(inProgressQuests)
                .Select(q => new SimpleListItemData()
                {
                    Identifier = q.Id,
                    Sprite = null,
                    Text = q.Title
                }), OnQuestInfoClicked, q => OnQuestActionClicked(q, canTakeQuests, canCompleteQuests), q => GetAvailableQuestViewTemplate(q, canTakeQuests, inProgressQuests, canCompleteQuests));

            _tabbedListViews.AddTab("Unavailable", "Unavailable", unavalableQuests
                .Select(q => new SimpleListItemData()
                {
                    Identifier = q.Id,
                    Sprite = null,
                    Text = q.Title
                }), OnQuestInfoClicked, null, _ => _unavailableQuestListItemViewTemplate);

            _tabbedListViews.AddTab("Completed", "Completed", completedQuests
                .Select(q => new SimpleListItemData()
                {
                    Identifier = q.Id,
                    Sprite = null,
                    Text = q.Title
                }), OnQuestInfoClicked, null, _ => _completedQuestListItemViewTemplate);

            _questsTabsController.SelectTab(currentTabIndex);
        }

        private SimpleListItemView GetAvailableQuestViewTemplate(SimpleListItemData itemData, QuestDefinition[] canTake, QuestDefinition[] inProgress, QuestDefinition[] canComplete)
        {
            if (canTake.Any(q => q.Id == itemData.Identifier))
            {
                return _canTakeQuestListItemViewTemplate;
            }
            else if (inProgress.Any(q => q.Id == itemData.Identifier))
            {
                return _inProgressQuestListItemViewTemplate;
            }
            else if (canComplete.Any(q => q.Id == itemData.Identifier))
            {
                return _canCompleteQuestListItemViewTemplate;
            }
            else
            {
                SLog.Error($"Quest with ID '{itemData.Identifier}' and Text '{itemData.Text}' is not in any of the available quest categories.");
                return _unavailableQuestListItemViewTemplate;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _tabbedListViews?.Clear();
        }

        private void OnQuestInfoClicked(string questId)
        {
            QuestInfoClicked?.Invoke(questId);
        }

        private void OnQuestActionClicked(string questId, QuestDefinition[] canTake, QuestDefinition[] canComplete)
        {
            if (canTake.Any(q => q.Id == questId))
            {
                QuestAcceptClicked?.Invoke(questId);
            }
            else if (canComplete.Any(q => q.Id == questId))
            {
                QuestCompleteClicked?.Invoke(questId);
            }
        }
    }
}
