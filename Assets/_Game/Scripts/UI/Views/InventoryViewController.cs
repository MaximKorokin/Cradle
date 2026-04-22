using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.Items.Traits;
using Assets._Game.Scripts.UI.DataAggregators;
using System;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class InventoryViewController : IDisposable
    {
        private readonly InventoryView _inventoryView;

        private IInventoryHudData _inventoryHudData;

        public event Action<InventorySlot> SlotClick
        {
            add => _inventoryView.SlotClick += value;
            remove => _inventoryView.SlotClick -= value;
        }

        public InventoryViewController(InventoryView inventoryView)
        {
            _inventoryView = inventoryView;

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

        public void Bind(IInventoryHudData inventoryHudData)
        {
            _inventoryHudData = inventoryHudData;
            _inventoryHudData.Changed += Redraw;
        }

        public void Unbind()
        {
            if (_inventoryHudData != null)
                _inventoryHudData.Changed -= Redraw;
            _inventoryHudData = null;
        }

        public void Redraw()
        {
            _inventoryView.Render(_inventoryHudData);
        }

        private void OnFilterByArmorButtonClicked(bool isOn)
        {
            _inventoryHudData.SetEnumerationFilter(!isOn ? null : item =>
            {
                if (item == null || !item.Value.Definition.TryGetTrait<EquippableTrait>(out var equippableTrait))
                    return false;

                return equippableTrait.Slot is EquipmentSlotType.Armor or EquipmentSlotType.Helmet or EquipmentSlotType.Gloves or EquipmentSlotType.Boots;
            });
        }

        private void OnFilterByWeaponButtonClicked(bool isOn)
        {
            _inventoryHudData.SetEnumerationFilter(!isOn ? null : item =>
            {
                if (item == null || !item.Value.Definition.TryGetTrait<EquippableTrait>(out var equippableTrait))
                    return false;

                return equippableTrait.Slot is EquipmentSlotType.Weapon;
            });
        }

        private void OnFilterByConsumableButtonClicked(bool isOn)
        {
            _inventoryHudData.SetEnumerationFilter(!isOn ? null : item =>
            {
                if (item == null || !item.Value.Definition.TryGetTrait<UsableTrait>(out var consumableTrait) || !consumableTrait.Consumable)
                    return false;

                return true;
            });
        }

        private void OnFilterByResourceButtonClicked(bool isOn)
        {
            _inventoryHudData.SetEnumerationFilter(!isOn ? null : item =>
            {
                if (item == null)
                    return false;

                // todo: add "ResourceTrait" or something similar to mark resource items instead of checking for the absence of traits
                return item.Value.Definition.Traits.Length == 0;
            });
        }

        private void OnOrderByNameButtonClicked()
        {
        }

        private void OnOrderByTypeButtonClicked()
        {
        }
    }
}
