using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.DataFormatters;
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
        private RectTransform _requirementsInfo;
        [SerializeField]
        private TMP_Text _requirementsText;
        [SerializeField]
        private RectTransform _objectivesInfo;
        [SerializeField]
        private TMP_Text _objectivesText;
        [SerializeField]
        private RectTransform _rewardsInfo;
        [SerializeField]
        private TMP_Text _experienceRewardText;
        [SerializeField]
        private TMP_Text _itemRewardsText;

        protected override void Render(QuestDescriptionViewData viewData)
        {
            var quest = viewData.QuestData;

            if (_titleText != null)
                _titleText.text = quest.Title;

            RenderDescription(quest);
            RenderRequirements(quest);
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

        private void RenderRequirements(QuestStateDisplayData quest)
        {
            var hasRequirements = _requirementsText != null && !string.IsNullOrWhiteSpace(quest.RequirementsText);

            if (_requirementsInfo != null)
                _requirementsInfo.gameObject.SetActive(hasRequirements);

            if (_requirementsText != null)
                _requirementsText.text = hasRequirements ? quest.RequirementsText : string.Empty;
        }

        private void RenderRewards(QuestStateDisplayData quest)
        {
            var hasExperienceRewards = _experienceRewardText != null && !string.IsNullOrWhiteSpace(quest.ExperienceRewardText);
            var hasItemsRewards = _itemRewardsText != null && !string.IsNullOrWhiteSpace(quest.ItemRewardsText);

            _rewardsInfo.gameObject.SetActive(hasExperienceRewards || hasItemsRewards);

            if (hasExperienceRewards)
                _experienceRewardText.text = quest.ExperienceRewardText;

            if (hasItemsRewards)
                _itemRewardsText.text = quest.ItemRewardsText;
        }
    }
}
