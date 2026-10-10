using Assets._Game.Scripts.Quests;
using System.Text;

namespace Assets._Game.Scripts.UI.DataFormatters
{
    public sealed class QuestStateFormatter : IDataFormatter<QuestStateSnapshot, QuestStateDisplayData>
    {
        private readonly QuestObjectiveProgressFormatter _questObjectiveProgressFormatter;

        public QuestStateFormatter(QuestObjectiveProgressFormatter questObjectiveProgressFormatter)
        {
            _questObjectiveProgressFormatter = questObjectiveProgressFormatter;
        }

        public QuestStateDisplayData FormatData(QuestStateSnapshot data)
        {
            var stringBuilder = new StringBuilder();
            var objectives = data.GetObjectives();
            for (int i = 0; i < objectives.Length; i++)
            {
                stringBuilder.AppendLine($"- {_questObjectiveProgressFormatter.FormatData(objectives[i])}");
            }
            var objectivesText = stringBuilder.ToString();

            stringBuilder.Clear();
            if (data.Definition.Reward != null)
            {
                for (int i = 0; i < data.Definition.Reward.ItemRewards.Length; i++)
                {
                    stringBuilder.AppendLine($"- {data.Definition.Reward.ItemRewards[i].Name}");
                }
            }
            var itemRewardsText = stringBuilder.ToString();
            var experienceRewardText = data.Definition.Reward != null ? $"{data.Definition.Reward.Experience} Exp" : "";

            return new QuestStateDisplayData(
                data.Definition.Title,
                data.Definition.Description,
                $"Level: {data.Definition.RequiredLevel}+",
                objectivesText,
                experienceRewardText,
                itemRewardsText);
        }
    }

    public readonly struct QuestStateDisplayData
    {
        public readonly bool HasData;

        public readonly string Title;
        public readonly string Description;
        public readonly string RequirementsText;
        public readonly string ObjectivesText;
        public readonly string ExperienceRewardText;
        public readonly string ItemRewardsText;

        public QuestStateDisplayData(
            string title,
            string description,
            string requirementsText,
            string objectivesText,
            string experienceRewardText,
            string itemRewardsText)
        {
            HasData = true;

            Title = title;
            Description = description;
            RequirementsText = requirementsText;
            ObjectivesText = objectivesText;
            ExperienceRewardText = experienceRewardText;
            ItemRewardsText = itemRewardsText;
        }
    }
}
