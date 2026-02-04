using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.UI.Views;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public class InventoryEquipmentWindow : UIWindow
    {
        [SerializeField]
        private InventoryView _inventoryView;
        [SerializeField]
        private EquipmentView _equipmentView;

        public event Action<int> InventorySlotPointerDown;
        public event Action<int> InventorySlotPointerUp;
        public event Action<EquipmentSlotType> EquipmentSlotPointerDown;
        public event Action<EquipmentSlotType> EquipmentSlotPointerUp;

        public override bool IsModal => true;

        public override void OnShow()
        {
            _inventoryView.SlotPointerDown += OnInventorySlotPointerDown;
            _inventoryView.SlotPointerUp += OnInventorySlotPointerUp;
            _equipmentView.SlotPointerDown += OnEquipmentSlotPointerDown;
            _equipmentView.SlotPointerUp += OnEquipmentSlotPointerUp;
        }

        public override void OnHide()
        {
            _inventoryView.SlotPointerDown -= OnInventorySlotPointerDown;
            _inventoryView.SlotPointerUp -= OnInventorySlotPointerUp;
            _equipmentView.SlotPointerDown -= OnEquipmentSlotPointerDown;
            _equipmentView.SlotPointerUp -= OnEquipmentSlotPointerUp;
        }

        public void Render(InventoryModel inventoryModel, EquipmentModel equipmentModel)
        {
            _inventoryView.Render(inventoryModel);
            _inventoryView.Bind();
            _equipmentView.Render(equipmentModel);
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

        private void OnEquipmentSlotPointerDown(EquipmentSlotType slotType)
        {
            EquipmentSlotPointerDown?.Invoke(slotType);
        }

        private void OnEquipmentSlotPointerUp(EquipmentSlotType slotType)
        {
            EquipmentSlotPointerUp?.Invoke(slotType);
        }
    }
}
