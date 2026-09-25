using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.DataAggregators;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class InventoryViewController : ViewControllerBase<InventoryView>
    {
        private readonly IGlobalEventBus _globalEventBus;

        private IInventoryHudData _inventoryHudData;

        public InventoryViewController(IGlobalEventBus globalEventBus)
        {
            _globalEventBus = globalEventBus;
        }

        public override void Initialize(InventoryView view)
        {
            base.Initialize(view);

            View.FilterByClothingButtonClicked += OnFilterByClothingButtonClicked;
            View.FilterByWeaponButtonClicked += OnFilterByWeaponButtonClicked;
            View.FilterByUtilityButtonClicked += OnFilterByUtilityButtonClicked;
            View.FilterByResourceButtonClicked += OnFilterByResourceButtonClicked;

            View.OrderByNameButtonClicked += OnOrderByNameButtonClicked;
            View.OrderByPurposeButtonClicked += OnOrderByPurposeButtonClicked;
        }

        public override void Dispose()
        {
            View.FilterByClothingButtonClicked -= OnFilterByClothingButtonClicked;
            View.FilterByWeaponButtonClicked -= OnFilterByWeaponButtonClicked;
            View.FilterByUtilityButtonClicked -= OnFilterByUtilityButtonClicked;
            View.FilterByResourceButtonClicked -= OnFilterByResourceButtonClicked;

            View.OrderByNameButtonClicked -= OnOrderByNameButtonClicked;
            View.OrderByPurposeButtonClicked -= OnOrderByPurposeButtonClicked;

            base.Dispose();
        }

        public void Bind(IInventoryHudData inventoryHudData)
        {
            _inventoryHudData = inventoryHudData;
            _inventoryHudData.Changed += OnInventoryChanged;
        }

        public void Unbind()
        {
            if (_inventoryHudData != null)
            {
                _inventoryHudData.Changed -= OnInventoryChanged;
                _inventoryHudData = null;
            }
        }

        protected override void OnRender()
        {
            View.RequestRender(_inventoryHudData);
        }

        private void OnInventoryChanged()
        {
            Render();
        }

        private void OnFilterButtonClicked(bool isOn, ItemStackPurpose purpose)
        {
            _inventoryHudData.SetEnumerationFilter(!isOn
                ? null
                : item => item != null && item.Value.GetPurpose() == purpose);
        }

        private void OnFilterByClothingButtonClicked(bool isOn) => OnFilterButtonClicked(isOn, ItemStackPurpose.Clothing);
        private void OnFilterByWeaponButtonClicked(bool isOn) => OnFilterButtonClicked(isOn, ItemStackPurpose.Weapon);
        private void OnFilterByUtilityButtonClicked(bool isOn) => OnFilterButtonClicked(isOn, ItemStackPurpose.Utility);
        private void OnFilterByResourceButtonClicked(bool isOn) => OnFilterButtonClicked(isOn, ItemStackPurpose.Resource);

        private void OnOrderByNameButtonClicked()
        {
            _globalEventBus.Publish(new InventorySortRequest(InventorySortingType.ByName, _inventoryHudData.InventoryModel));
        }

        private void OnOrderByPurposeButtonClicked()
        {
            _globalEventBus.Publish(new InventorySortRequest(InventorySortingType.ByPurpose, _inventoryHudData.InventoryModel));
        }
    }
}
