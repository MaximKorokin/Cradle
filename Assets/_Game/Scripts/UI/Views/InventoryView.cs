using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.UI.DataAggregators;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class InventoryView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _inventorySlotsParent;
        [SerializeField]
        private InventorySlotView _inventorySlotTemplate;
        [Space]
        [SerializeField]
        private TMP_Text _weightText;
        [SerializeField]
        private TMP_Text _goldText;
        [SerializeField]
        private TMP_Text _pneumaText;
        [SerializeField]
        private TMP_Text _slotsAmountText;
        [Space]
        [SerializeField]
        private Toggle _filterByClothingToggle;
        [SerializeField]
        private Toggle _filterByWeaponToggle;
        [SerializeField]
        private Toggle _filterByUtilityToggle;
        [SerializeField]
        private Toggle _filterByResourceToggle;
        [Space]
        [SerializeField]
        private Button _orderByNameButton;
        [SerializeField]
        private Button _orderByPurposeButton;

        private readonly List<InventorySlotView> _slots = new();

        private IInventoryHudData _inventoryHudData;

        public event Action<InventorySlot> SlotClick;

        public event Action<bool> FilterByClothingButtonClicked;
        public event Action<bool> FilterByWeaponButtonClicked;
        public event Action<bool> FilterByUtilityButtonClicked;
        public event Action<bool> FilterByResourceButtonClicked;

        public event Action OrderByNameButtonClicked;
        public event Action OrderByPurposeButtonClicked;

        private void OnEnable()
        {
            _filterByClothingToggle.onValueChanged.AddListener(isOn => FilterByClothingButtonClicked?.Invoke(isOn));
            _filterByWeaponToggle.onValueChanged.AddListener(isOn => FilterByWeaponButtonClicked?.Invoke(isOn));
            _filterByUtilityToggle.onValueChanged.AddListener(isOn => FilterByUtilityButtonClicked?.Invoke(isOn));
            _filterByResourceToggle.onValueChanged.AddListener(isOn => FilterByResourceButtonClicked?.Invoke(isOn));

            _orderByNameButton.onClick.AddListener(() => OrderByNameButtonClicked?.Invoke());
            _orderByPurposeButton.onClick.AddListener(() => OrderByPurposeButtonClicked?.Invoke());
        }

        private void OnDisable()
        {
            _filterByClothingToggle.onValueChanged.RemoveAllListeners();
            _filterByWeaponToggle.onValueChanged.RemoveAllListeners();
            _filterByUtilityToggle.onValueChanged.RemoveAllListeners();
            _filterByResourceToggle.onValueChanged.RemoveAllListeners();

            _orderByNameButton.onClick.RemoveAllListeners();
            _orderByPurposeButton.onClick.RemoveAllListeners();
        }

        public void Render(IInventoryHudData inventoryHudData)
        {
            _inventorySlotTemplate.gameObject.SetActive(false);

            _inventoryHudData = inventoryHudData;
            foreach (var slot in _slots)
            {
                slot.gameObject.SetActive(false);
            }

            // Render inventory slots
            foreach (var (inventorySlot, stack) in _inventoryHudData.Enumerate())
            {
                if (_slots.Count > inventorySlot.Index)
                {
                    var slot = _slots[inventorySlot.Index];
                    slot.Render(stack);
                    slot.gameObject.SetActive(true);
                    continue;
                }

                // Instantiate new slot if there are not enough in the pool
                var newSlot = Instantiate(_inventorySlotTemplate, _inventorySlotsParent);
                newSlot.Bind(inventorySlot);
                newSlot.PointerClick += OnSlotPointerClick;
                _slots.Add(newSlot);
                newSlot.gameObject.SetActive(true);
                newSlot.Render(stack);
            }

            // Update text fields
            _pneumaText.text = _inventoryHudData.ViewPneuma ? $"Pneuma: {_inventoryHudData.Pneuma}" : "";
            _goldText.text = _inventoryHudData.ViewGold ? $"Gold: {_inventoryHudData.Gold}" : "";
            _slotsAmountText.text = _inventoryHudData.ViewSlotsAmount ? $"Slots: {_inventoryHudData.SlotsUsed} / {_inventoryHudData.SlotsMax}" : "";
            _weightText.text = _inventoryHudData.ViewWeight ? $"Weight: {_inventoryHudData.WeightCurrent} / {_inventoryHudData.WeightMax}" : "";
        }

        public void Unbind()
        {
            _inventoryHudData = null;
        }

        private void OnSlotPointerClick(InventorySlot slotIndex)
        {
            SlotClick?.Invoke(slotIndex);
        }
    }
}
