using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Services;
using Assets._Game.Scripts.Items.Crafting;
using Assets._Game.Scripts.Items.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public sealed class CraftingHudData : DataAggregatorBase
    {
        private readonly EntityRepository _entityRepository;
        private readonly CraftingRecipeDefinitionCatalog _recipeDefinitionCatalog;
        private readonly CraftingService _craftingService;

        private InventoryModel _inventoryModel;

        public event Action Changed;

        public CraftingHudData(
            EntityRepository entityRepository,
            CraftingRecipeDefinitionCatalog recipeDefinitionCatalog,
            CraftingService craftingService)
        {
            _entityRepository = entityRepository;
            _recipeDefinitionCatalog = recipeDefinitionCatalog;
            _craftingService = craftingService;
        }

        public void SetInventoryEntity(string inventoryEntityId)
        {
            if (_entityRepository.Get(inventoryEntityId).TryGetModule<InventoryModule>(out var inventoryModule))
            {
                if (_inventoryModel != null)
                {
                    _inventoryModel.Changed -= OnInventoryChanged;
                }
                _inventoryModel = inventoryModule.Inventory;
                _inventoryModel.Changed += OnInventoryChanged;
                Changed?.Invoke();
            }
        }

        public IEnumerable<CraftingRecipeDefinition> AvailableRecipes
        {
            get
            {
                if (_inventoryModel == null)
                    return Enumerable.Empty<CraftingRecipeDefinition>();

                return _recipeDefinitionCatalog.Where(recipe => _craftingService.CanCraftAny(recipe, _inventoryModel));
            }
        }

        public IEnumerable<CraftingRecipeDefinition> UnavailableRecipes
        {
            get
            {
                if (_inventoryModel == null)
                    return _recipeDefinitionCatalog;

                return _recipeDefinitionCatalog.Where(recipe => !_craftingService.CanCraftAny(recipe, _inventoryModel));
            }
        }

        private void OnInventoryChanged()
        {
            Changed?.Invoke();
        }

        public override void Dispose()
        {
            base.Dispose();

            if (_inventoryModel != null)
            {
                _inventoryModel.Changed -= OnInventoryChanged;
            }
        }
    }
}
