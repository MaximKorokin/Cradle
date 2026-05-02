using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Crafting;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.DataAggregators;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class CraftingWindowController : WindowControllerBase<CraftingWindow, EmptyWindowControllerArguments>
    {
        private CraftingWindow _window;

        private readonly WindowManager _windowManager;
        private readonly CraftingHudData _craftingHudData;
        private readonly PlayerContext _playerContext;
        private readonly ItemStackFactory _itemStackFactory;

        public CraftingWindowController(
            WindowManager windowManager,
            CraftingHudData craftingHudData,
            PlayerContext playerContext,
            ItemStackFactory itemStackFactory)
        {
            _windowManager = windowManager;
            _craftingHudData = craftingHudData;
            _playerContext = playerContext;
            _itemStackFactory = itemStackFactory;
        }

        public override void Bind(CraftingWindow window)
        {
            _window = window;
            window.RecipeClicked += OnRecipeClicked;

            _window.Render(_craftingHudData);
        }

        public override void Unbind()
        {
            _window.RecipeClicked -= OnRecipeClicked;
            _window.Clear();
        }

        private void OnRecipeClicked(CraftingRecipeDefinition recipe)
        {
            if (!_playerContext.Player.TryGetModule<InventoryModule>(out var inventoryModule))
                return;

            var maxCraftable = CalculateMaxCraftable(recipe, inventoryModule);
            if (maxCraftable == 0)
                return;

            var maxResultAmount = recipe.Result.Item.MaxAmount;
            var maxAmount = System.Math.Min(maxCraftable, maxResultAmount);

            if (maxAmount == 1)
            {
                CraftRecipe(recipe, 1, inventoryModule);
            }
            else
            {
                _windowManager.ShowAmountPicker(1, maxAmount, amount =>
                {
                    CraftRecipe(recipe, amount, inventoryModule);
                });
            }
        }

        private int CalculateMaxCraftable(CraftingRecipeDefinition recipe, InventoryModule inventoryModule)
        {
            if (recipe.Ingredients.Length == 0)
                return int.MaxValue;

            var maxCraftable = int.MaxValue;
            foreach (var ingredient in recipe.Ingredients)
            {
                var key = ItemKey.From(ingredient.Item, null);
                var available = inventoryModule.Inventory.Count(key);
                var possibleCrafts = available / ingredient.Amount;
                maxCraftable = System.Math.Min(maxCraftable, possibleCrafts);
            }

            return maxCraftable;
        }

        private void CraftRecipe(CraftingRecipeDefinition recipe, int craftCount, InventoryModule inventoryModule)
        {
            // Remove ingredients
            foreach (var ingredient in recipe.Ingredients)
            {
                var key = ItemKey.From(ingredient.Item, null);
                var totalToRemove = ingredient.Amount * craftCount;
                inventoryModule.Inventory.Remove(key, totalToRemove);
            }

            // Add result
            var totalResultAmount = recipe.Result.Amount * craftCount;
            var resultSnapshot = _itemStackFactory.Create(recipe.Result.Item.Id, totalResultAmount).Snapshot;
            inventoryModule.Inventory.Add(resultSnapshot);
        }
    }
}
