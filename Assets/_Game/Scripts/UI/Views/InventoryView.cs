using Assets._Game.Scripts.Items.Inventory;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class InventoryView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _inventorySlotsParent;
        [SerializeField]
        private InventorySlotView _inventorySlotTemplate;

        private readonly List<(int Index, InventorySlotView Slot)> _slots = new();

        public event Action<int> SlotPointerDown;
        public event Action<int> SlotPointerUp;

        public void Bind()
        {
            foreach (var (index, slot) in _slots)
            {
                slot.Bind(index);
                slot.PointerDown -= OnSlotPointerDown;
                slot.PointerDown += OnSlotPointerDown;
                slot.PointerUp -= OnSlotPointerUp;
                slot.PointerUp += OnSlotPointerUp;
            }
        }

        public void Render(InventoryModel inventoryModel)
        {
            Clear();

            _inventorySlotTemplate.gameObject.SetActive(false);
            foreach (var (index, stack) in inventoryModel.Enumerate())
            {
                var slotView = Instantiate(_inventorySlotTemplate, _inventorySlotsParent);
                _slots.Add((index, slotView));
                slotView.gameObject.SetActive(true);
                if (slotView != null)
                {
                    slotView.Render(stack);
                }
            }
        }

        public void Clear()
        {
            foreach (var slot in _slots)
            {
                Destroy(slot.Slot.gameObject);
            }
            _slots.Clear();
        }

        private void OnSlotPointerDown(int slotIndex)
        {
            SlotPointerDown?.Invoke(slotIndex);
        }

        private void OnSlotPointerUp(int slotIndex)
        {
            SlotPointerUp?.Invoke(slotIndex);
        }
    }
}
