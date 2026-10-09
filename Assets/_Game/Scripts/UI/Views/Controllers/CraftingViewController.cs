using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Services;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Items.Crafting;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.DataAggregators;

namespace Assets._Game.Scripts.UI.Views.Controllers
{
    public sealed class CraftingViewController : ViewControllerBase<CraftingView, CraftingViewData>
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly CraftingService _craftingService;

        public CraftingViewController(
            IGlobalEventBus globalEventBus,
            CraftingService craftingService)
        {
            _globalEventBus = globalEventBus;
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
            var maxCraftable = _craftingService.CalculateMaxCraftable(recipe, Data.InventoryModel);
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
                selectedAmount => _globalEventBus.Publish(new CraftRequest(Data.InventoryPath, recipe.Id, selectedAmount)));
        }
    }
}
