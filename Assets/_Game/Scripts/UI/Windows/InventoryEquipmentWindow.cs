using Assets._Game.Scripts.Items.Equipment;
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

        public event Action<int> InventorySlotPointerDown;
        public event Action<int> InventorySlotPointerUp;
        public event Action<EquipmentSlotKey> EquipmentSlotPointerDown;
        public event Action<EquipmentSlotKey> EquipmentSlotPointerUp;

        public override void OnShow()
        {
            _inventoryView.SlotPointerDown += OnInventorySlotPointerDown;
            _inventoryView.SlotPointerUp += OnInventorySlotPointerUp;
            _equipmentView.SlotPointerDown += OnEquipmentSlotPointerDown;
            _equipmentView.SlotPointerUp += OnEquipmentSlotPointerUp;
        }

        public override void OnHide()
        {
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

        private void OnInventorySlotPointerDown(int slotIndex)
        {
            InventorySlotPointerDown?.Invoke(slotIndex);
        }

        private void OnInventorySlotPointerUp(int slotIndex)
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
