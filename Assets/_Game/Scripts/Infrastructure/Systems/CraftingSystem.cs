using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Services;
using Assets._Game.Scripts.Infrastructure.Systems.GameAnalytics;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Crafting;
using Assets._Game.Scripts.Items.Inventory;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class CraftingSystem : SystemBase
    {
        private readonly ItemContainerResolver _itemContainerResolver;
        private readonly CraftingService _craftingService;
        private readonly CraftingRecipeDefinitionCatalog _craftingRecipeCatalogue;
        private readonly EntityRepository _entityRepository;

        public CraftingSystem(
            IGlobalEventBus globalEventBus,
            EntityRepository entityRepository,
            ItemContainerResolver itemContainerResolver,
            CraftingService craftingService,
            CraftingRecipeDefinitionCatalog craftingRecipeCatalogue) : base(globalEventBus)
        {
            _entityRepository = entityRepository;
            _itemContainerResolver = itemContainerResolver;
            _craftingService = craftingService;
            _craftingRecipeCatalogue = craftingRecipeCatalogue;

            TrackGlobalEvent<CraftRequest>(HandleCraftRequest);
        }

        private void HandleCraftRequest(CraftRequest request)
        {
            var inventoryModel = _itemContainerResolver.ResolveContainer<InventoryModel>(request.InventoryPath);

            var recipe = _craftingRecipeCatalogue.Get(request.RecipeId);
            if (recipe == null)
            {
                return;
            }

            if (!_craftingService.CanCraft(recipe, request.Amount, inventoryModel))
            {
                return;
            }

            _craftingService.Craft(recipe, request.Amount, inventoryModel);

            _entityRepository.TryGet(request.InventoryPath.EntityId, out var entity);
            GlobalEventBus.Publish(new GameAnalyticsEvent(AnalyticsEntryType.ItemCraft, entity, null, request.Amount, recipe.name));
            foreach (var ingredient in recipe.Ingredients)
                GlobalEventBus.Publish(new GameAnalyticsEvent(AnalyticsEntryType.ItemDisappear, entity, null, ingredient.Amount * request.Amount, ingredient.Item.Name));
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
