using Assets._Game.Scripts.Quests;
using System.Text;
using TMPro;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class QuestDescriptionWindow : UIWindowBase
    {
        [SerializeField]
        private TMP_Text _titleText;
        [SerializeField]
        private TMP_Text _descriptionText;
        [SerializeField]
        private TMP_Text _objectivesText;
        [SerializeField]
        private TMP_Text _rewardText;

        public void Render(QuestDefinition quest)
        {
            if (_titleText != null)
                _titleText.text = quest.Title;

            if (_objectivesText != null)
                _objectivesText.text = BuildObjectivesText(quest);

            if (_rewardText != null)
                _rewardText.text = BuildRewardText(quest);
        }

        private string BuildObjectivesText(QuestDefinition quest)
        {
            if (quest.Objectives == null || quest.Objectives.Length == 0)
                return string.Empty;

            var sb = new StringBuilder();
            foreach (var objective in quest.Objectives)
            {
                sb.AppendLine($"- {objective.GetType().Name.Replace("ObjectiveDefinition", string.Empty)} x{objective.RequiredAmount}");
            }
            return sb.ToString().TrimEnd();
        }

        private string BuildRewardText(QuestDefinition quest)
        {
            if (quest.Reward == null) return string.Empty;

            var sb = new StringBuilder();
            if (quest.Reward.Experience > 0)
                sb.AppendLine($"Experience: {quest.Reward.Experience}");

            if (quest.Reward.ItemRewards != null && quest.Reward.ItemRewards.Length > 0)
            {
                foreach (var item in quest.Reward.ItemRewards)
                    sb.AppendLine($"Item: {item.name}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
