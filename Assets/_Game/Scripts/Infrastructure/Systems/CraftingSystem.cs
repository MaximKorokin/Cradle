using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Services;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Crafting;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class CraftingSystem : SystemBase
    {
        private readonly ItemContainerResolver _itemContainerResolver;
        private readonly CraftingService _craftingService;
        private readonly CraftingRecipeDefinitionCatalog _craftingRecipeCatalogue;

        public CraftingSystem(
            IGlobalEventBus globalEventBus,
            ItemContainerResolver itemContainerResolver,
            CraftingService craftingService,
            CraftingRecipeDefinitionCatalog craftingRecipeCatalogue) : base(globalEventBus)
        {
            _itemContainerResolver = itemContainerResolver;
            _craftingService = craftingService;
            _craftingRecipeCatalogue = craftingRecipeCatalogue;

            TrackGlobalEvent<CraftRequest>(HandleCraftRequest);
        }

        private void HandleCraftRequest(CraftRequest request)
        {
            var inventoryModel = _itemContainerResolver.ResolveInventory(request.InventoryPath);

            var recipe = _craftingRecipeCatalogue.Get(request.RecipeId);
            if (recipe == null)
            {
                return;
            }

            _craftingService.Craft(recipe, request.Amount, inventoryModel);
        }
    }

    public readonly struct CraftRequest : IGlobalEvent
    {
        public ItemContainerPath InventoryPath { get; }
        public string RecipeId { get; }
        public int Amount { get; }

        public CraftRequest(ItemContainerPath inventoryPath, string recipeId, int amount)
        {
            InventoryPath = inventoryPath;
            RecipeId = recipeId;
            Amount = amount;
        }
    }
}
