using Assets._Game.Scripts.Shared.Unity;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.DataFormatters;
using Assets._Game.Scripts.UI.Views.Widgets;
using TMPro;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class QuestDescriptionView : UIViewBase<QuestDescriptionViewData>
    {
        [SerializeField]
        private TMP_Text _titleText;
        [SerializeField]
        private RectTransform _descriptionInfo;
        [SerializeField]
        private TMP_Text _descriptionText;
        [SerializeField]
        private TMP_Text _levelRequirementText;
        [SerializeField]
        private TMP_Text _completionText;
        [SerializeField]
        private RectTransform _objectivesInfo;
        [SerializeField]
        private TMP_Text _objectivesText;
        [SerializeField]
        private RectTransform _rewardsInfo;
        [SerializeField]
        private TMP_Text _experienceRewardText;
        [SerializeField]
        private RectTransform _itemRewardsParent;
        [SerializeField]
        private ItemStackWidget _itemRewardTemplate;

        private readonly GameObjectCollection _instantiatedObjects = new();

        protected override void Awake()
        {
            base.Awake();

            _itemRewardTemplate.gameObject.SetActive(false);
        }

        protected override void Render(QuestDescriptionViewData viewData)
        {
            Clear();

            var quest = viewData.QuestData;

            if (_titleText != null)
                _titleText.text = quest.Title;

            RenderDescription(quest);
            RenderMetaData(quest);
            RenderObjectives(quest);
            RenderRewards(quest);
        }

        private void RenderDescription(QuestStateDisplayData quest)
        {
            var hasDescription = _descriptionText != null && !string.IsNullOrWhiteSpace(quest.Description);

            _descriptionInfo.gameObject.SetActive(hasDescription);

            if (hasDescription)
                _descriptionText.text = quest.Description;
        }

        private void RenderObjectives(QuestStateDisplayData quest)
        {
            var hasObjectives = _objectivesText != null && !string.IsNullOrWhiteSpace(quest.ObjectivesText);

            _objectivesInfo.gameObject.SetActive(hasObjectives);

            if (hasObjectives)
                _objectivesText.text = quest.ObjectivesText;
        }

        private void RenderMetaData(QuestStateDisplayData quest)
        {
            var hasLevelRequirement = _levelRequirementText != null && !string.IsNullOrWhiteSpace(quest.LevelRequirementText);
            _levelRequirementText.gameObject.SetActive(hasLevelRequirement);
            _levelRequirementText.text = hasLevelRequirement ? quest.LevelRequirementText : string.Empty;

            _completionText.text = quest.CompletionText;
        }

        private void RenderRewards(QuestStateDisplayData quest)
        {
            var hasExperienceRewards = _experienceRewardText != null && !string.IsNullOrWhiteSpace(quest.ExperienceRewardText);
            var hasItemRewards = quest.ItemRewards != null && quest.ItemRewards.Length > 0;

            _rewardsInfo.gameObject.SetActive(hasExperienceRewards || hasItemRewards);

            if (hasExperienceRewards)
                _experienceRewardText.text = quest.ExperienceRewardText;

            if (_itemRewardTemplate != null && _itemRewardsParent != null)
            {
                foreach (var item in quest.ItemRewards)
                {
                    var itemWidget = _instantiatedObjects.Instantiate(_itemRewardTemplate, _itemRewardsParent);
                    itemWidget.gameObject.SetActive(true);
                    itemWidget.Render(item);
                }
            }
        }

        private void Clear()
        {
            _descriptionText.text = string.Empty;
            _levelRequirementText.text = string.Empty;
            _objectivesText.text = string.Empty;
            _experienceRewardText.text = string.Empty;
            _instantiatedObjects.Clear();
        }
    }
}
