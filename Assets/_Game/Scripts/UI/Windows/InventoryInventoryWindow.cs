using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.UI.Views;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public class InventoryInventoryWindow : UIWindow
    {
        [SerializeField]
        private InventoryView _firstInventoryView;
        [SerializeField]
        private InventoryView _secondInventoryView;
        [SerializeField]
        private InventorySlotView _inventorySlotTemplate;

        public event Action<int> FirstInventorySlotPointerDown;
        public event Action<int> FirstInventorySlotPointerUp;
        public event Action<int> SecondInventorySlotPointerDown;
        public event Action<int> SecondInventorySlotPointerUp;

        public override void OnShow()
        {
            _inventorySlotTemplate.gameObject.SetActive(false);

            _firstInventoryView.SlotPointerDown += OnFirstInventorySlotPointerDown;
            _firstInventoryView.SlotPointerUp += OnFirstInventorySlotPointerUp;
            _secondInventoryView.SlotPointerDown += OnSecondInventorySlotPointerDown;
            _secondInventoryView.SlotPointerUp += OnSecondInventorySlotPointerUp;
        }

        public override void OnHide()
        {
            _firstInventoryView.SlotPointerDown -= OnFirstInventorySlotPointerDown;
            _firstInventoryView.SlotPointerUp -= OnFirstInventorySlotPointerUp;
            _secondInventoryView.SlotPointerDown -= OnSecondInventorySlotPointerDown;
            _secondInventoryView.SlotPointerUp -= OnSecondInventorySlotPointerUp;
        }

        public void Render(InventoryModel firstInventory, InventoryModel secondInventory)
        {
            _firstInventoryView.Render(firstInventory);
            _firstInventoryView.Bind();
            _secondInventoryView.Render(secondInventory);
            _secondInventoryView.Bind();
        }

        private void OnFirstInventorySlotPointerDown(int slotIndex)
        {
            FirstInventorySlotPointerDown?.Invoke(slotIndex);
        }

        private void OnFirstInventorySlotPointerUp(int slotIndex)
        {
            FirstInventorySlotPointerUp?.Invoke(slotIndex);
        }

        private void OnSecondInventorySlotPointerDown(int slotIndex)
        {
            SecondInventorySlotPointerDown?.Invoke(slotIndex);
        }

        private void OnSecondInventorySlotPointerUp(int slotIndex)
        {
            SecondInventorySlotPointerUp?.Invoke(slotIndex);
        }
    }
}
