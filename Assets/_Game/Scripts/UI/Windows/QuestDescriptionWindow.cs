using Assets._Game.Scripts.UI.DataFormatters;
using TMPro;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class QuestDescriptionWindow : UIWindowBase
    {
        [SerializeField]
        private TMP_Text _titleText;
        [SerializeField]
        private RectTransform _descriptionInfo;
        [SerializeField]
        private TMP_Text _descriptionText;
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

        public void Render(QuestStateDisplayData quest)
        {
            if (_titleText != null)
                _titleText.text = quest.Title;

            RenderDescription(quest);
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

        private void RenderRewards(QuestStateDisplayData quest)
        {
            var hasExperienceRewards = _experienceRewardText != null && !string.IsNullOrWhiteSpace(quest.ExperienceRewardText);
            var hasItemsRewards = _itemRewardsText != null && !string.IsNullOrWhiteSpace(quest.ItemRewardsText);

            _objectivesInfo.gameObject.SetActive(hasExperienceRewards || hasItemsRewards);

            if (hasExperienceRewards)
                _experienceRewardText.text = quest.ExperienceRewardText;

            if (hasItemsRewards)
                _itemRewardsText.text = quest.ItemRewardsText;
        }
    }
}
