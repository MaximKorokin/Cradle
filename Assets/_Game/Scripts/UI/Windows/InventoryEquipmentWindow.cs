using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public class InventoryEquipmentWindow : UIWindowBase
    {
        [SerializeField]
        private InventoryView _inventoryView;
        [SerializeField]
        private EquipmentView _equipmentView;

        public event Action<InventorySlot> InventorySlotPointerDown;
        public event Action<InventorySlot> InventorySlotPointerUp;
        public event Action<EquipmentSlotKey> EquipmentSlotPointerDown;
        public event Action<EquipmentSlotKey> EquipmentSlotPointerUp;

        public override void OnShow()
        {
            base.OnShow();

            _inventoryView.SlotPointerDown += OnInventorySlotPointerDown;
            _inventoryView.SlotPointerUp += OnInventorySlotPointerUp;
            _equipmentView.SlotPointerDown += OnEquipmentSlotPointerDown;
            _equipmentView.SlotPointerUp += OnEquipmentSlotPointerUp;
        }

        public override void OnHide()
        {
            base.OnHide();

            _inventoryView.Unbind();
            _equipmentView.Unbind();

            _inventoryView.SlotPointerDown -= OnInventorySlotPointerDown;
            _inventoryView.SlotPointerUp -= OnInventorySlotPointerUp;
            _equipmentView.SlotPointerDown -= OnEquipmentSlotPointerDown;
            _equipmentView.SlotPointerUp -= OnEquipmentSlotPointerUp;
        }

        public void Render(IInventoryHudData inventoryHudData, IEquipmentHudData equipmentHudData)
        {
            _inventoryView.Render(inventoryHudData);
            _equipmentView.Render(equipmentHudData);
            _equipmentView.Bind();
        }

        private void OnInventorySlotPointerDown(InventorySlot slotIndex)
        {
            InventorySlotPointerDown?.Invoke(slotIndex);
        }

        private void OnInventorySlotPointerUp(InventorySlot slotIndex)
        {
            InventorySlotPointerUp?.Invoke(slotIndex);
        }

        private void OnEquipmentSlotPointerDown(EquipmentSlotKey slot)
        {
            EquipmentSlotPointerDown?.Invoke(slot);
        }

        private void OnEquipmentSlotPointerUp(EquipmentSlotKey slot)
        {
            EquipmentSlotPointerUp?.Invoke(slot);
        }
    }
}
