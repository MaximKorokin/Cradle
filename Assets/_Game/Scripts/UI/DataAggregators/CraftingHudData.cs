using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Crafting;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public sealed class CraftingHudData : DataAggregatorBase
    {
        private readonly CraftingRecipeDefinitionCatalog _recipeDefinitionCatalog;
        private readonly PlayerContext _playerContext;

        public CraftingHudData(
            CraftingRecipeDefinitionCatalog recipeDefinitionCatalog,
            PlayerContext playerContext)
        {
            _recipeDefinitionCatalog = recipeDefinitionCatalog;
            _playerContext = playerContext;
        }

        public IEnumerable<CraftingRecipeDefinition> AvailableRecipes
        {
            get
            {
                if (!_playerContext.Player.TryGetModule<InventoryModule>(out var inventoryModule))
                    return Enumerable.Empty<CraftingRecipeDefinition>();

                return _recipeDefinitionCatalog.Where(recipe => CanCraft(recipe, inventoryModule));
            }
        }

        public IEnumerable<CraftingRecipeDefinition> UnavailableRecipes
        {
            get
            {
                if (!_playerContext.Player.TryGetModule<InventoryModule>(out var inventoryModule))
                    return _recipeDefinitionCatalog;

                return _recipeDefinitionCatalog.Where(recipe => !CanCraft(recipe, inventoryModule));
            }
        }

        private bool CanCraft(CraftingRecipeDefinition recipe, InventoryModule inventoryModule)
        {
            foreach (var ingredient in recipe.Ingredients)
            {
                var key = ItemKey.From(ingredient.Item, null);
                var count = inventoryModule.Inventory.Count(key);
                if (count < ingredient.Amount)
                    return false;
            }
            return true;
        }
    }
}
