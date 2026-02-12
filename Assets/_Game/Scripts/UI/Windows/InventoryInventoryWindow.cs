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

        public event Action<int> FirstInventorySlotPointerDown;
        public event Action<int> FirstInventorySlotPointerUp;
        public event Action<int> SecondInventorySlotPointerDown;
        public event Action<int> SecondInventorySlotPointerUp;

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
