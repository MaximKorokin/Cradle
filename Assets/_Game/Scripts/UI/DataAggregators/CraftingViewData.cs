using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Services;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Crafting;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.Shared;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public sealed class CraftingViewData : EntityBoundDataAggregatorBase
    {
        private readonly CraftingService _craftingService;
        private readonly InventoryViewData _inventoryViewData;

        private CraftingModule _crafterCraftingModule;

        public ItemContainerPath InventoryPath => _inventoryViewData.ContainerPath;
        public InventoryModel InventoryModel => _inventoryViewData.InventoryModel;

        public CraftingViewData(
            EntityRepository entityRepository,
            CraftingService craftingService,
            InventoryViewData inventoryViewData) : base(entityRepository)
        {
            _craftingService = craftingService;
            _inventoryViewData = inventoryViewData;
            _inventoryViewData.Changed += OnInventoryChanged;
            _inventoryViewData.Invalidated += OnInventoryInvalidated;
        }

        public void SetConsumerEntity(IReadOnlyObservableData<EntryRef> consumerEntityId)
        {
            _inventoryViewData.SetEntityId(consumerEntityId);
        }

        protected override void OnBoundEntityChanged()
        {
            _crafterCraftingModule = Entity?.GetModule<CraftingModule>();
            NotifyChanged();
        }

        private void OnInventoryChanged()
        {
            NotifyChanged();
        }

        private void OnInventoryInvalidated()
        {
            NotifyInvalidated();
        }

        public override void Dispose()
        {
            _inventoryViewData.Changed -= OnInventoryChanged;
            _inventoryViewData.Invalidated -= OnInventoryInvalidated;
            _inventoryViewData.Dispose();
            base.Dispose();
        }

        public IEnumerable<CraftingRecipeDefinition> AvailableRecipes
        {
            get
            {
                if (_inventoryViewData.InventoryModel == null || _crafterCraftingModule == null)
                    return Enumerable.Empty<CraftingRecipeDefinition>();

                return _crafterCraftingModule.Recipes.Where(recipe => _craftingService.CanCraftAny(recipe, _inventoryViewData.InventoryModel));
            }
        }

        public IEnumerable<CraftingRecipeDefinition> UnavailableRecipes
        {
            get
            {
                if (_inventoryViewData.InventoryModel == null || _crafterCraftingModule == null)
                    return Enumerable.Empty<CraftingRecipeDefinition>();

                return _crafterCraftingModule.Recipes.Where(recipe => !_craftingService.CanCraftAny(recipe, _inventoryViewData.InventoryModel));
            }
        }
    }
}
