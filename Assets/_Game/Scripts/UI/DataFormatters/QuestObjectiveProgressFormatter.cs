using Assets._Game.Scripts.Quests.Objectives;

namespace Assets._Game.Scripts.UI.DataFormatters
{
    public sealed class QuestObjectiveProgressFormatter : IDataFormatter<ObjectiveProgressSnapshot, string>
    {
        public string FormatData(ObjectiveProgressSnapshot data)
        {
            return data.ObjectiveDefinition switch
            {
                ItemsInInventoryObjectiveDefinition itemsObjective => $"({data.CurrentAmount} / {itemsObjective.RequiredAmount}) {itemsObjective.Item.Name}",
                EntityKillsObjectiveDefinition entityKillsObjective => $"({data.CurrentAmount} / {entityKillsObjective.RequiredAmount}) {entityKillsObjective.Entity.DisplayName}",
                LevelObjectiveDefinition levelObjective => $"({data.CurrentAmount} / {levelObjective.RequiredAmount}) Level",

                // Unsupported type fallback
                ObjectiveDefinition objective => $"({data.CurrentAmount} / {objective.RequiredAmount}) {objective.GetType()}",
                _ => "unsupported objective progress type"
            };
        }
    }
}
