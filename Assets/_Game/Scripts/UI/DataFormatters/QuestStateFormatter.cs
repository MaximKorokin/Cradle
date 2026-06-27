using Assets._Game.Scripts.Quests;
using System.Text;

namespace Assets._Game.Scripts.UI.DataFormatters
{
    public sealed class QuestStateFormatter : IDataFormatter<QuestState, QuestStateDisplayData>
    {
        private readonly QuestObjectiveProgressFormatter _questObjectiveProgressFormatter;

        public QuestStateFormatter(QuestObjectiveProgressFormatter questObjectiveProgressFormatter)
        {
            _questObjectiveProgressFormatter = questObjectiveProgressFormatter;
        }

        public QuestStateDisplayData FormatData(QuestState data)
        {
            var stringBuilder = new StringBuilder();
            for (int i = 0; i < data.Objectives.Length; i++)
            {
                stringBuilder.AppendLine($"- {_questObjectiveProgressFormatter.FormatData(data.Objectives[i])}");
            }
            var objectivesText = stringBuilder.ToString();

            stringBuilder.Clear();
            for (int i = 0; i < data.Definition.Reward.ItemRewards.Length; i++)
            {
                stringBuilder.AppendLine($"- {data.Definition.Reward.ItemRewards[i].Name}");
            }
            var itemRewardsText = stringBuilder.ToString();

            return new QuestStateDisplayData(
                data.Definition.Title,
                data.Definition.Description,
                objectivesText,
                $"{data.Definition.Reward.Experience} Exp",
                itemRewardsText);
        }
    }

    public readonly struct QuestStateDisplayData
    {
        public readonly bool HasData;

        public readonly string Title;
        public readonly string Description;
        public readonly string ObjectivesText;
        public readonly string ExperienceRewardText;
        public readonly string ItemRewardsText;

        public QuestStateDisplayData(
            string title,
            string description,
            string objectivesText,
            string experienceRewardText,
            string itemRewardsText)
        {
            HasData = true;

            Title = title;
            Description = description;
            ObjectivesText = objectivesText;
            ExperienceRewardText = experienceRewardText;
            ItemRewardsText = itemRewardsText;
        }
    }
}
