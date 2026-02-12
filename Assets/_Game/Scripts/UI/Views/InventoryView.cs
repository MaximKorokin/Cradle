using Assets._Game.Scripts.UI.DataAggregators;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class InventoryView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _inventorySlotsParent;
        [SerializeField]
        private InventorySlotView _inventorySlotTemplate;
        [SerializeField]
        private TMP_Text _weightText;
        [SerializeField]
        private TMP_Text _goldText;

        private readonly List<InventorySlotView> _slots = new();

        private IInventoryHudData _inventoryHudData;

        public event Action<int> SlotPointerDown;
        public event Action<int> SlotPointerUp;

        public void Render(IInventoryHudData inventoryHudData)
        {
            _inventorySlotTemplate.gameObject.SetActive(false);

            _inventoryHudData = inventoryHudData;
            inventoryHudData.Changed += OnInventoryHudDataChanged;
            OnInventoryHudDataChanged();
        }

        private void OnInventoryHudDataChanged()
        {
            foreach (var slot in _slots)
            {
                slot.gameObject.SetActive(false);
            }

            foreach (var (index, stack) in _inventoryHudData.InventoryModel.Enumerate())
            {
                if (_slots.Count > index)
                {
                    var slot = _slots[index];
                    slot.Render(stack);
                    slot.gameObject.SetActive(true);
                    continue;
                }

                var slotView = Instantiate(_inventorySlotTemplate, _inventorySlotsParent);
                slotView.Bind(index);
                slotView.PointerDown += OnSlotPointerDown;
                slotView.PointerUp += OnSlotPointerUp;
                _slots.Add(slotView);
                slotView.gameObject.SetActive(true);
                slotView.Render(stack);
            }

            _weightText.text = _inventoryHudData.ViewWeight ? $"{_inventoryHudData.WeightCurrent}kg / {_inventoryHudData.WeightMax}kg" : "";
            _goldText.text = _inventoryHudData.ViewGold ? $"{_inventoryHudData.Gold} gold" : "";
        }

        public void Unbind()
        {
            if (_inventoryHudData != null)
            {
                _inventoryHudData.Changed -= OnInventoryHudDataChanged;
                _inventoryHudData = null;
            }
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
