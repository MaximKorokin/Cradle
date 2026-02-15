using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.Items;
using System.Collections.Generic;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public sealed class CheatsHudData
    {
        private readonly ItemDefinitionCatalog _itemDefinitionCatalog;
        private readonly StatusEffectDefinitionCatalog _statusEffectDefinitionCatalog;

        public CheatsHudData(
            ItemDefinitionCatalog itemDefinitionCatalog,
            StatusEffectDefinitionCatalog statusEffectDefinitionCatalog)
        {
            _itemDefinitionCatalog = itemDefinitionCatalog;
            _statusEffectDefinitionCatalog = statusEffectDefinitionCatalog;
        }

        public IEnumerable<ItemDefinition> ItemDefinitions => _itemDefinitionCatalog;

        public IEnumerable<StatusEffectDefinition> StatusEffectDefinitions => _statusEffectDefinitionCatalog;
    }
}
