using Assets._Game.Scripts.Quests.Objectives;

namespace Assets._Game.Scripts.UI.DataFormatters
{
    public sealed class QuestObjectiveProgressFormatter : IDataFormatter<ObjectiveProgress, string>
    {
        public string FormatData(ObjectiveProgress data)
        {
            return data switch
            {
                ItemsInInventoryObjectiveProgress itemsObjective => $"({itemsObjective.CurrentAmount} / {itemsObjective.RequiredAmount}) {itemsObjective.Definition.Item.Name}",
                EntityKillsObjectiveProgress entityKillsObjective => $"({entityKillsObjective.CurrentAmount} / {entityKillsObjective.RequiredAmount}) {entityKillsObjective.Definition.Entity.DisplayName}",
                LevelObjectiveProgress levelObjective => $"({levelObjective.CurrentAmount} / {levelObjective.RequiredAmount}) Level",
                
                // Unsupported type fallback
                ObjectiveProgress objective => $"({objective.CurrentAmount} / {objective.RequiredAmount}) {objective.GetType()}",
                _ => "unsupported objective progress type"
            };
        }
    }
}
