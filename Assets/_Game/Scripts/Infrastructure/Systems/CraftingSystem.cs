using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Services;
using Assets._Game.Scripts.Items.Crafting;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class CraftingSystem : SystemBase
    {
        private readonly IPlayerProvider _playerProvider;
        private readonly CraftingService _craftingService;
        private readonly CraftingRecipeDefinitionCatalog _craftingRecipeCatalogue;

        public CraftingSystem(
            IGlobalEventBus globalEventBus,
            IPlayerProvider playerProvider,
            CraftingService craftingService,
            CraftingRecipeDefinitionCatalog craftingRecipeCatalogue) : base(globalEventBus)
        {
            _playerProvider = playerProvider;
            _craftingService = craftingService;
            _craftingRecipeCatalogue = craftingRecipeCatalogue;

            TrackGlobalEvent<CraftRequest>(HandleCraftRequest);
        }

        private void HandleCraftRequest(CraftRequest request)
        {
            if (!_playerProvider.Player.TryGetModule<InventoryModule>(out var inventoryModule))
            {
                return;
            }

            var recipe = _craftingRecipeCatalogue.Get(request.RecipeId);
            if (recipe == null)
            {
                return;
            }

            _craftingService.Craft(recipe, request.Amount, inventoryModule);
        }
    }

    public readonly struct CraftRequest : IGlobalEvent
    {
        public string RecipeId { get; }
        public int Amount { get; }

        public CraftRequest(string recipeId, int amount)
        {
            RecipeId = recipeId;
            Amount = amount;
        }
    }
}
