using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Services;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Crafting;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.Shared;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public sealed class CraftingWindowData : ItemContainerDataAggregatorBase
    {
        private readonly CraftingService _craftingService;

        private CraftingModule _crafterCraftingModule;

        private InventoryModel InventoryModel => ItemContainer as InventoryModel;

        public CraftingWindowData(
            ItemContainerResolver itemContainerResolver,
            EntityRepository entityRepository,
            CraftingService craftingService) : base(itemContainerResolver, entityRepository)
        {
            _craftingService = craftingService;
        }

        public void SetCrafterEntity(IReadOnlyObservableData<string> crafterEntityId)
        {
            if (EntityRepository.Get(crafterEntityId.Value).TryGetModule<CraftingModule>(out var craftingModule))
            {
                _crafterCraftingModule = craftingModule;
                NotifyChanged();
            }
        }

        public IEnumerable<CraftingRecipeDefinition> AvailableRecipes
        {
            get
            {
                if (InventoryModel == null || _crafterCraftingModule == null)
                    return Enumerable.Empty<CraftingRecipeDefinition>();

                return _crafterCraftingModule.Recipes.Where(recipe => _craftingService.CanCraftAny(recipe, InventoryModel));
            }
        }

        public IEnumerable<CraftingRecipeDefinition> UnavailableRecipes
        {
            get
            {
                if (InventoryModel == null || _crafterCraftingModule == null)
                    return Enumerable.Empty<CraftingRecipeDefinition>();

                return _crafterCraftingModule.Recipes.Where(recipe => !_craftingService.CanCraftAny(recipe, InventoryModel));
            }
        }
        
        protected override ItemContainerPath GetContainerPath(string entityId) => ItemContainerPath.Inventory(entityId);
    }
}
