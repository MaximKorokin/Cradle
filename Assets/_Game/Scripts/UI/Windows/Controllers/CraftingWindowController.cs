using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Services;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Crafting;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class CraftingWindowController : WindowControllerBase<CraftingWindow, CraftingWindowControllerArguments>
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly ItemContainerResolver _itemContainerResolver;
        private readonly CraftingViewData _craftingViewData;
        private readonly CraftingService _craftingService;
        private readonly CraftingViewController _craftingViewController;

        public CraftingWindowController(
            IGlobalEventBus globalEventBus,
            ItemContainerResolver itemContainerResolver,
            CraftingViewData craftingViewData,
            CraftingService craftingService,
            CraftingViewController craftingViewController)
        {
            _globalEventBus = globalEventBus;
            _itemContainerResolver = itemContainerResolver;
            _craftingViewData = craftingViewData;
            _craftingService = craftingService;
            _craftingViewController = craftingViewController;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _craftingViewData.SetCrafterEntity(Arguments.CrafterEntityId);
            _craftingViewData.SetEntityId(Arguments.InventoryEntityId);
        }

        protected override void OnBind()
        {
            base.OnBind();

            _craftingViewController.Initialize(Window.CraftingView);
            _craftingViewController.Bind(_craftingViewData);

            Window.CraftingView.RecipeActionClicked += OnRecipeActionClicked;
        }

        protected override void OnUnbind()
        {
            base.OnUnbind();

            Window.CraftingView.RecipeActionClicked -= OnRecipeActionClicked;

            _craftingViewController.Unbind();
        }

        private void OnRecipeActionClicked(CraftingRecipeDefinition recipe)
        {
            var inventoryPath = ItemContainerPath.Inventory(Arguments.InventoryEntityId.Value);
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
                selectedAmount =>
                {
                    _globalEventBus.Publish(new CraftRequest(inventoryPath, recipe.Id, selectedAmount));
                });
        }

        protected override void Redraw()
        {
            _craftingViewController.Render();
        }

        public override void Dispose()
        {
            base.Dispose();

            _craftingViewData.Dispose();
            _craftingViewController.Dispose();
        }
    }

    public readonly struct CraftingWindowControllerArguments : IWindowControllerArguments
    {
        public IReadOnlyObservableData<string> CrafterEntityId { get; }
        public IReadOnlyObservableData<string> InventoryEntityId { get; }
        public IReadOnlyObservableData<string> EquipmentEntityId { get; }

        public CraftingWindowControllerArguments(
            IReadOnlyObservableData<string> crafterEntityId,
            IReadOnlyObservableData<string> inventoryEntityId,
            IReadOnlyObservableData<string> equipmentEntityId)
        {
            CrafterEntityId = crafterEntityId;
            InventoryEntityId = inventoryEntityId;
            EquipmentEntityId = equipmentEntityId;
        }
    }
}
