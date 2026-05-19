using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Services;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Crafting;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Services;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class CraftingWindowController : WindowControllerBase<CraftingWindow, CraftingWindowControllerArguments>
    {
        private CraftingWindow _window;

        private readonly IGlobalEventBus _globalEventBus;
        private readonly ItemContainerResolver _itemContainerResolver;
        private readonly WindowManager _windowManager;
        private readonly CraftingHudData _craftingHudData;
        private readonly EquipmentHudData _equipmentHudData;
        private readonly CraftingService _craftingService;
        private readonly ItemPreviewService _itemPreviewService;

        public CraftingWindowController(
            IGlobalEventBus globalEventBus,
            ItemContainerResolver itemContainerResolver,
            WindowManager windowManager,
            CraftingHudData craftingHudData,
            EquipmentHudData equipmentHudData,
            CraftingService craftingService,
            ItemPreviewService itemPreviewService)
        {
            _globalEventBus = globalEventBus;
            _itemContainerResolver = itemContainerResolver;
            _windowManager = windowManager;
            _craftingHudData = craftingHudData;
            _equipmentHudData = equipmentHudData;
            _craftingService = craftingService;
            _itemPreviewService = itemPreviewService;
        }

        public override void Initialize(CraftingWindowControllerArguments arguments)
        {
            base.Initialize(arguments);

            _craftingHudData.SetInventoryEntity(arguments.InventoryEntityId);
            _equipmentHudData.SetEquipmentEntity(arguments.EquipmentEntityId);
        }

        public override void Bind(CraftingWindow window)
        {
            _window = window;
            _window.RecipeInfoClicked += OnRecipeInfoClicked;
            _window.RecipeActionClicked += OnRecipeActionClicked;
            _craftingHudData.Changed += OnCraftingDataChanged;

            _window.Render(_craftingHudData);
        }

        public override void Unbind()
        {
            _window.RecipeInfoClicked -= OnRecipeInfoClicked;
            _window.RecipeActionClicked -= OnRecipeActionClicked;
            _craftingHudData.Changed -= OnCraftingDataChanged;
            _window.Clear();
        }

        private void OnCraftingDataChanged()
        {
            _window.Render(_craftingHudData);
        }

        private void OnRecipeInfoClicked(CraftingRecipeDefinition recipe)
        {
            _itemPreviewService.ShowItemDefinitionPreview(
                recipe.Result.ItemDefinition,
                ItemContainerPath.Equipment(Arguments.EquipmentEntityId),
                _equipmentHudData.EquipmentModel.FindOccupiedSlotForItem(recipe.Result.ItemDefinition));
        }

        private void OnRecipeActionClicked(CraftingRecipeDefinition recipe)
        {
            var inventoryPath = ItemContainerPath.Inventory(Arguments.InventoryEntityId);
            var inventoryModel = _itemContainerResolver.ResolveInventory(inventoryPath);

            var maxCraftable = _craftingService.CalculateMaxCraftable(recipe, inventoryModel);
            if (maxCraftable == 0)
                return;

            var maxResultAmount = recipe.Result.ItemDefinition.MaxAmount;
            var maxAmount = System.Math.Min(maxCraftable, maxResultAmount);

            _windowManager.ShowAmountPickerIfNeeded(maxAmount, maxAmount, amount =>
            {
                _globalEventBus.Publish(new CraftRequest(inventoryPath, recipe.Id, amount));
            });
        }
    }

    public readonly struct CraftingWindowControllerArguments : IWindowControllerArguments
    {
        public string CrafterEntityId { get; }
        public string InventoryEntityId { get; }
        public string EquipmentEntityId { get; }

        public CraftingWindowControllerArguments(string crafterEntityId, string inventoryEntityId, string equipmentEntityId)
        {
            CrafterEntityId = crafterEntityId;
            InventoryEntityId = inventoryEntityId;
            EquipmentEntityId = equipmentEntityId;
        }
    }
}
