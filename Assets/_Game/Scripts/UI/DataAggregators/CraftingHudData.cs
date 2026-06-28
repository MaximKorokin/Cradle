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
        private readonly CraftingService _craftingService;

        private InventoryModel _inventoryModel;
        private CraftingModule _crafterCraftingModule;

        public event Action Changed;

        public CraftingHudData(
            EntityRepository entityRepository,
            CraftingService craftingService)
        {
            _entityRepository = entityRepository;
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

        public void SetCrafterEntity(string crafterEntityId)
        {
            if (_entityRepository.Get(crafterEntityId).TryGetModule<CraftingModule>(out var craftingModule))
            {
                _crafterCraftingModule = craftingModule;
                Changed?.Invoke();
            }
        }

        public IEnumerable<CraftingRecipeDefinition> AvailableRecipes
        {
            get
            {
                if (_inventoryModel == null || _crafterCraftingModule == null)
                    return Enumerable.Empty<CraftingRecipeDefinition>();

                return _crafterCraftingModule.Recipes.Where(recipe => _craftingService.CanCraftAny(recipe, _inventoryModel));
            }
        }

        public IEnumerable<CraftingRecipeDefinition> UnavailableRecipes
        {
            get
            {
                if (_inventoryModel == null || _crafterCraftingModule == null)
                    return Enumerable.Empty<CraftingRecipeDefinition>();

                return _crafterCraftingModule.Recipes.Where(recipe => !_craftingService.CanCraftAny(recipe, _inventoryModel));
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
