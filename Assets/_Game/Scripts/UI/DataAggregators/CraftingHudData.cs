using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Services;
using Assets._Game.Scripts.Items.Crafting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public sealed class CraftingHudData : DataAggregatorBase
    {
        private readonly CraftingRecipeDefinitionCatalog _recipeDefinitionCatalog;
        private readonly IPlayerProvider _playerProvider;
        private readonly CraftingService _craftingService;

        public event Action Changed;

        public CraftingHudData(
            CraftingRecipeDefinitionCatalog recipeDefinitionCatalog,
            IPlayerProvider playerProvider,
            CraftingService craftingService)
        {
            _recipeDefinitionCatalog = recipeDefinitionCatalog;
            _playerProvider = playerProvider;
            _craftingService = craftingService;

            // Subscribe to inventory changes
            if (_playerProvider.Player.TryGetModule<InventoryModule>(out var inventoryModule))
            {
                inventoryModule.Inventory.Changed += OnInventoryChanged;
            }
        }

        public IEnumerable<CraftingRecipeDefinition> AvailableRecipes
        {
            get
            {
                if (!_playerProvider.Player.TryGetModule<InventoryModule>(out var inventoryModule))
                    return Enumerable.Empty<CraftingRecipeDefinition>();

                return _recipeDefinitionCatalog.Where(recipe => _craftingService.CanCraftAny(recipe, inventoryModule));
            }
        }

        public IEnumerable<CraftingRecipeDefinition> UnavailableRecipes
        {
            get
            {
                if (!_playerProvider.Player.TryGetModule<InventoryModule>(out var inventoryModule))
                    return _recipeDefinitionCatalog;

                return _recipeDefinitionCatalog.Where(recipe => !_craftingService.CanCraftAny(recipe, inventoryModule));
            }
        }

        private void OnInventoryChanged()
        {
            Changed?.Invoke();
        }

        public override void Dispose()
        {
            base.Dispose();

            if (_playerProvider.Player.TryGetModule<InventoryModule>(out var inventoryModule))
            {
                inventoryModule.Inventory.Changed -= OnInventoryChanged;
            }
        }
    }
}
