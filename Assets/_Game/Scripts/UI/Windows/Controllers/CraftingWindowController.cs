using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Services;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Crafting;
using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.DataAggregators;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class CraftingWindowController : WindowControllerBase<CraftingWindow, CraftingWindowControllerArguments>
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly ItemContainerResolver _itemContainerResolver;
        private readonly CraftingHudData _craftingHudData;
        private readonly EquipmentHudData _equipmentHudData;
        private readonly CraftingService _craftingService;

        public CraftingWindowController(
            IGlobalEventBus globalEventBus,
            ItemContainerResolver itemContainerResolver,
            CraftingHudData craftingHudData,
            EquipmentHudData equipmentHudData,
            CraftingService craftingService)
        {
            _globalEventBus = globalEventBus;
            _itemContainerResolver = itemContainerResolver;
            _craftingHudData = craftingHudData;
            _equipmentHudData = equipmentHudData;
            _craftingService = craftingService;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _craftingHudData.SetCrafterEntity(Arguments.CrafterEntityId);
            _craftingHudData.SetEntityId(Arguments.InventoryEntityId);
            _equipmentHudData.SetEntityId(Arguments.EquipmentEntityId);
        }

        protected override void OnBind()
        {
            base.OnBind();

            _craftingHudData.Changed += Redraw;

            Window.RecipeInfoClicked += OnRecipeInfoClicked;
            Window.RecipeActionClicked += OnRecipeActionClicked;
        }

        protected override void OnUnbind()
        {
            base.OnUnbind();

            _craftingHudData.Changed -= Redraw;

            Window.RecipeInfoClicked -= OnRecipeInfoClicked;
            Window.RecipeActionClicked -= OnRecipeActionClicked;
        }

        private void OnRecipeInfoClicked(CraftingRecipeDefinition recipe)
        {

        }

        private void OnRecipeActionClicked(CraftingRecipeDefinition recipe)
        {
            var inventoryPath = ItemContainerPath.Inventory(Arguments.InventoryEntityId.Value);
            var inventoryModel = _itemContainerResolver.ResolveInventory(inventoryPath);

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
            Window.Render(_craftingHudData);
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
