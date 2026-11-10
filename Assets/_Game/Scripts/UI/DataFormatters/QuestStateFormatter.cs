using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Quests;
using System.Linq;
using System.Text;

namespace Assets._Game.Scripts.UI.DataFormatters
{
    public sealed class QuestStateFormatter : IDataFormatter<QuestStateSnapshot, QuestStateDisplayData>
    {
        private readonly QuestObjectiveProgressFormatter _questObjectiveProgressFormatter;
        private readonly ItemStackFactory _itemStackFactory;

        public QuestStateFormatter(
            QuestObjectiveProgressFormatter questObjectiveProgressFormatter,
            ItemStackFactory itemStackFactory)
        {
            _questObjectiveProgressFormatter = questObjectiveProgressFormatter;
            _itemStackFactory = itemStackFactory;
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

            var experienceRewardText = data.Definition.Reward != null ? $"{data.Definition.Reward.Experience} Exp" : "";
            var itemRewards = data.Definition.Reward.ItemRewards.Select(item => _itemStackFactory.Create(item.Id, 1).Snapshot).ToArray();

            return new QuestStateDisplayData(
                data.Definition.Title,
                data.Definition.Description,
                $"Level: {data.Definition.RequiredLevel}+",
                objectivesText,
                experienceRewardText,
                itemRewards);
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
        public readonly ItemStackSnapshot[] ItemRewards;

        public QuestStateDisplayData(
            string title,
            string description,
            string requirementsText,
            string objectivesText,
            string experienceRewardText,
            ItemStackSnapshot[] itemRewards)
        {
            HasData = true;

            Title = title;
            Description = description;
            RequirementsText = requirementsText;
            ObjectivesText = objectivesText;
            ExperienceRewardText = experienceRewardText;
            ItemRewards = itemRewards;
        }
    }
}
