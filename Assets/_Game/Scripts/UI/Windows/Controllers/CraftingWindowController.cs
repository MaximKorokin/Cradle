using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Services;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Items.Crafting;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Services;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class CraftingWindowController : WindowControllerBase<CraftingWindow, EmptyWindowControllerArguments>
    {
        private CraftingWindow _window;

        private readonly IGlobalEventBus _globalEventBus;
        private readonly WindowManager _windowManager;
        private readonly CraftingHudData _craftingHudData;
        private readonly EquipmentHudData _equipmentHudData;
        private readonly IPlayerProvider _playerProvider;
        private readonly CraftingService _craftingService;
        private readonly ItemPreviewService _itemPreviewService;

        public CraftingWindowController(
            IGlobalEventBus globalEventBus,
            WindowManager windowManager,
            CraftingHudData craftingHudData,
            EquipmentHudData equipmentHudData,
            IPlayerProvider playerProvider,
            CraftingService craftingService,
            ItemPreviewService itemPreviewService)
        {
            _globalEventBus = globalEventBus;
            _windowManager = windowManager;
            _craftingHudData = craftingHudData;
            _equipmentHudData = equipmentHudData;
            _playerProvider = playerProvider;
            _craftingService = craftingService;
            _itemPreviewService = itemPreviewService;
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
                _equipmentHudData.EquipmentModel.FindOccupiedSlotForItem(recipe.Result.ItemDefinition));
        }

        private void OnRecipeActionClicked(CraftingRecipeDefinition recipe)
        {
            if (!_playerProvider.Player.TryGetModule<InventoryModule>(out var inventoryModule))
                return;

            var maxCraftable = _craftingService.CalculateMaxCraftable(recipe, inventoryModule);
            if (maxCraftable == 0)
                return;

            var maxResultAmount = recipe.Result.ItemDefinition.MaxAmount;
            var maxAmount = System.Math.Min(maxCraftable, maxResultAmount);

            if (maxAmount == 1)
            {
                _globalEventBus.Publish(new CraftRequest(recipe.Id, 1));
            }
            else
            {
                _windowManager.ShowAmountPicker(1, maxAmount, amount =>
                {
                    _globalEventBus.Publish(new CraftRequest(recipe.Id, amount));
                });
            }
        }
    }
}
