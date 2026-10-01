using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Services;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Crafting;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.DataAggregators;

namespace Assets._Game.Scripts.UI.Views.Controllers
{
    public sealed class CraftingViewController : ViewControllerBase<CraftingView, CraftingViewData>
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly ItemContainerResolver _itemContainerResolver;
        private readonly CraftingService _craftingService;

        public CraftingViewController(
            IGlobalEventBus globalEventBus,
            ItemContainerResolver itemContainerResolver,
            CraftingService craftingService)
        {
            _globalEventBus = globalEventBus;
            _itemContainerResolver = itemContainerResolver;
            _craftingService = craftingService;
        }

        public override void Initialize(CraftingView view)
        {
            base.Initialize(view);
            View.RecipeActionClicked += OnRecipeActionClicked;
        }

        public override void Dispose()
        {
            View.RecipeActionClicked -= OnRecipeActionClicked;
            base.Dispose();
        }

        private void OnRecipeActionClicked(CraftingRecipeDefinition recipe)
        {
            var inventoryPath = ItemContainerPath.Inventory(Data.EntityId);
            var inventoryModel = _itemContainerResolver.ResolveContainer<InventoryModel>(inventoryPath);

            var maxCraftable = _craftingService.CalculateMaxCraftable(recipe, inventoryModel);
            if (maxCraftable == 0)
                return;

            var maxResultAmount = recipe.Result.ItemDefinition.MaxAmount;
            var maxAmount = System.Math.Min(maxCraftable, maxResultAmount);

            WindowUtils.ShowAmountPickerThenConfirmation(
                _globalEventBus,
                maxAmount,
                maxAmount,
                "Confirm Crafting",
                amount => $"Craft {amount}x {recipe.Result.ItemDefinition.Name}?",
                selectedAmount => _globalEventBus.Publish(new CraftRequest(inventoryPath, recipe.Id, selectedAmount)));
        }
    }
}
