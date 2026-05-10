using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Quests;
using System.Collections.Generic;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public sealed class CheatsHudData : DataAggregatorBase
    {
        private readonly ItemDefinitionCatalog _itemDefinitionCatalog;
        private readonly StatusEffectDefinitionCatalog _statusEffectDefinitionCatalog;
        private readonly QuestDefinitionCatalog _questDefinitionCatalog;

        public CheatsHudData(
            ItemDefinitionCatalog itemDefinitionCatalog,
            StatusEffectDefinitionCatalog statusEffectDefinitionCatalog,
            QuestDefinitionCatalog questDefinitionCatalog)
        {
            _itemDefinitionCatalog = itemDefinitionCatalog;
            _statusEffectDefinitionCatalog = statusEffectDefinitionCatalog;
            _questDefinitionCatalog = questDefinitionCatalog;
        }

        public IEnumerable<ItemDefinition> ItemDefinitions => _itemDefinitionCatalog;

        public IEnumerable<StatusEffectDefinition> StatusEffectDefinitions => _statusEffectDefinitionCatalog;

        public IEnumerable<QuestDefinition> QuestDefinitions => _questDefinitionCatalog;
    }
}
