using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.DataAggregators;

namespace Assets._Game.Scripts.UI.Views.Controllers
{
    public sealed class InventoryViewController : ViewControllerBase<InventoryView>
    {
        private readonly IGlobalEventBus _globalEventBus;

        private IInventoryViewData _inventoryViewData;

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

        public void Bind(IInventoryViewData inventoryViewData)
        {
            _inventoryViewData = inventoryViewData;
            _inventoryViewData.Changed += OnInventoryChanged;
        }

        public void Unbind()
        {
            if (_inventoryViewData != null)
            {
                _inventoryViewData.Changed -= OnInventoryChanged;
                _inventoryViewData = null;
            }
        }

        protected override void OnRender()
        {
            View.RequestRender(_inventoryViewData);
        }

        private void OnInventoryChanged()
        {
            Render();
        }

        private void OnFilterButtonClicked(bool isOn, ItemStackPurpose purpose)
        {
            _inventoryViewData.SetEnumerationFilter(!isOn
                ? null
                : item => item != null && item.Value.GetPurpose() == purpose);
        }

        private void OnFilterByClothingButtonClicked(bool isOn) => OnFilterButtonClicked(isOn, ItemStackPurpose.Clothing);
        private void OnFilterByWeaponButtonClicked(bool isOn) => OnFilterButtonClicked(isOn, ItemStackPurpose.Weapon);
        private void OnFilterByUtilityButtonClicked(bool isOn) => OnFilterButtonClicked(isOn, ItemStackPurpose.Utility);
        private void OnFilterByResourceButtonClicked(bool isOn) => OnFilterButtonClicked(isOn, ItemStackPurpose.Resource);

        private void OnOrderByNameButtonClicked()
        {
            _globalEventBus.Publish(new InventorySortRequest(InventorySortingType.ByName, _inventoryViewData.InventoryModel));
        }

        private void OnOrderByPurposeButtonClicked()
        {
            _globalEventBus.Publish(new InventorySortRequest(InventorySortingType.ByPurpose, _inventoryViewData.InventoryModel));
        }
    }
}
