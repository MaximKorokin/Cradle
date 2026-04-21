using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.UI.DataAggregators;
using System;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class InventoryViewController : IDisposable
    {
        private readonly InventoryView _inventoryView;
        private readonly InventoryModel _inventoryModel;

        public event Action<InventorySlot> SlotClick
        {
            add => _inventoryView.SlotClick += value;
            remove => _inventoryView.SlotClick -= value;
        }

        public InventoryViewController(
            InventoryView inventoryView,
            InventoryModel inventoryModel)
        {
            _inventoryView = inventoryView;
            _inventoryModel = inventoryModel;

            _inventoryView.FilterByArmorButtonClicked += OnFilterByArmorButtonClicked;
            _inventoryView.FilterByWeaponButtonClicked += OnFilterByWeaponButtonClicked;
            _inventoryView.FilterByConsumableButtonClicked += OnFilterByConsumableButtonClicked;
            _inventoryView.FilterByResourceButtonClicked += OnFilterByResourceButtonClicked;

            _inventoryView.OrderByNameButtonClicked += OnOrderByNameButtonClicked;
            _inventoryView.OrderByTypeButtonClicked += OnOrderByTypeButtonClicked;
        }

        public void Dispose()
        {
            _inventoryView.FilterByArmorButtonClicked -= OnFilterByArmorButtonClicked;
            _inventoryView.FilterByWeaponButtonClicked -= OnFilterByWeaponButtonClicked;
            _inventoryView.FilterByConsumableButtonClicked -= OnFilterByConsumableButtonClicked;
            _inventoryView.FilterByResourceButtonClicked -= OnFilterByResourceButtonClicked;

            _inventoryView.OrderByNameButtonClicked -= OnOrderByNameButtonClicked;
            _inventoryView.OrderByTypeButtonClicked -= OnOrderByTypeButtonClicked;
        }

        public void Render(InventoryHudData inventoryHudData)
        {
            _inventoryView.Render(inventoryHudData);
        }

        private void OnFilterByArmorButtonClicked()
        {
        }

        private void OnFilterByWeaponButtonClicked()
        {
        }

        private void OnFilterByConsumableButtonClicked()
        {
        }

        private void OnFilterByResourceButtonClicked()
        {
        }

        private void OnOrderByNameButtonClicked()
        {
        }

        private void OnOrderByTypeButtonClicked()
        {
        }
    }
}
