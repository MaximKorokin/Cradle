using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public class InventoryInventoryWindow : UIWindowBase
    {
        [SerializeField]
        private InventoryView _firstInventoryView;
        [SerializeField]
        private InventoryView _secondInventoryView;

        public event Action<InventorySlot> FirstInventorySlotPointerDown;
        public event Action<InventorySlot> FirstInventorySlotPointerUp;
        public event Action<InventorySlot> SecondInventorySlotPointerDown;
        public event Action<InventorySlot> SecondInventorySlotPointerUp;

        public override void OnShow()
        {
            _firstInventoryView.SlotPointerDown += OnFirstInventorySlotPointerDown;
            _firstInventoryView.SlotPointerUp += OnFirstInventorySlotPointerUp;
            _secondInventoryView.SlotPointerDown += OnSecondInventorySlotPointerDown;
            _secondInventoryView.SlotPointerUp += OnSecondInventorySlotPointerUp;
        }

        public override void OnHide()
        {
            _firstInventoryView.Unbind();
            _secondInventoryView.Unbind();

            _firstInventoryView.SlotPointerDown -= OnFirstInventorySlotPointerDown;
            _firstInventoryView.SlotPointerUp -= OnFirstInventorySlotPointerUp;
            _secondInventoryView.SlotPointerDown -= OnSecondInventorySlotPointerDown;
            _secondInventoryView.SlotPointerUp -= OnSecondInventorySlotPointerUp;
        }

        public void Render(IInventoryHudData firstInventoryHudData, IInventoryHudData secondInventoryHudData)
        {
            _firstInventoryView.Render(firstInventoryHudData);
            _secondInventoryView.Render(secondInventoryHudData);
        }

        private void OnFirstInventorySlotPointerDown(InventorySlot slotIndex)
        {
            FirstInventorySlotPointerDown?.Invoke(slotIndex);
        }

        private void OnFirstInventorySlotPointerUp(InventorySlot slotIndex)
        {
            FirstInventorySlotPointerUp?.Invoke(slotIndex);
        }

        private void OnSecondInventorySlotPointerDown(InventorySlot slotIndex)
        {
            SecondInventorySlotPointerDown?.Invoke(slotIndex);
        }

        private void OnSecondInventorySlotPointerUp(InventorySlot slotIndex)
        {
            SecondInventorySlotPointerUp?.Invoke(slotIndex);
        }
    }
}
