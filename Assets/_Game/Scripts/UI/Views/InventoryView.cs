using Assets._Game.Scripts.UI.Views.Widgets;
using Assets._Game.Scripts.UI.DataAggregators;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class InventoryView : UIViewBase<IInventoryViewData>
    {
        [SerializeField]
        private RectTransform _inventorySlotsParent;
        [SerializeField]
        private InventorySlotWidget _inventorySlotTemplate;
        [Space]
        [SerializeField]
        private TMP_Text _weightText;
        [SerializeField]
        private TMP_Text _goldText;
        [SerializeField]
        private TMP_Text _pneumaText;
        [SerializeField]
        private TMP_Text _slotsAmountText;
        [SerializeField]
        private RectTransform _enchantDropArea;
        [SerializeField]
        private RectTransform _destroyDropArea;
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

        private readonly List<InventorySlotWidget> _slots = new();

        public event Action<bool> FilterByClothingButtonClicked;
        public event Action<bool> FilterByWeaponButtonClicked;
        public event Action<bool> FilterByUtilityButtonClicked;
        public event Action<bool> FilterByResourceButtonClicked;

        public event Action OrderByNameButtonClicked;
        public event Action OrderByPurposeButtonClicked;

        private void OnEnable()
        {
            _filterByClothingToggle.onValueChanged.AddListener(OnFilterByClothingToggleValueChanged);
            _filterByWeaponToggle.onValueChanged.AddListener(OnFilterByWeaponToggleValueChanged);
            _filterByUtilityToggle.onValueChanged.AddListener(OnFilterByUtilityToggleValueChanged);
            _filterByResourceToggle.onValueChanged.AddListener(OnFilterByResourceToggleValueChanged);

            _orderByNameButton.onClick.AddListener(OnOrderByNameButtonClicked);
            _orderByPurposeButton.onClick.AddListener(OnOrderByPurposeButtonClicked);
        }

        private void OnDisable()
        {
            _filterByClothingToggle.onValueChanged.RemoveListener(OnFilterByClothingToggleValueChanged);
            _filterByWeaponToggle.onValueChanged.RemoveListener(OnFilterByWeaponToggleValueChanged);
            _filterByUtilityToggle.onValueChanged.RemoveListener(OnFilterByUtilityToggleValueChanged);
            _filterByResourceToggle.onValueChanged.RemoveListener(OnFilterByResourceToggleValueChanged);

            _orderByNameButton.onClick.RemoveListener(OnOrderByNameButtonClicked);
            _orderByPurposeButton.onClick.RemoveListener(OnOrderByPurposeButtonClicked);
        }

        private void OnFilterByClothingToggleValueChanged(bool isOn) => FilterByClothingButtonClicked?.Invoke(isOn);
        private void OnFilterByWeaponToggleValueChanged(bool isOn) => FilterByWeaponButtonClicked?.Invoke(isOn);
        private void OnFilterByUtilityToggleValueChanged(bool isOn) => FilterByUtilityButtonClicked?.Invoke(isOn);
        private void OnFilterByResourceToggleValueChanged(bool isOn) => FilterByResourceButtonClicked?.Invoke(isOn);

        private void OnOrderByNameButtonClicked() => OrderByNameButtonClicked?.Invoke();
        private void OnOrderByPurposeButtonClicked() => OrderByPurposeButtonClicked?.Invoke();

        protected override void Render(IInventoryViewData inventoryViewData)
        {
            _inventorySlotTemplate.gameObject.SetActive(false);

            foreach (var slot in _slots)
            {
                slot.gameObject.SetActive(false);
            }

            // Render inventory slots
            foreach (var (inventorySlot, stack) in inventoryViewData.Enumerate())
            {
                if (_slots.Count > inventorySlot.Index)
                {
                    var slot = _slots[inventorySlot.Index];
                    slot.Bind(inventoryViewData.ContainerPath, inventorySlot.ToInt64());
                    slot.Render(stack);
                    slot.gameObject.SetActive(true);
                    continue;
                }

                // Instantiate new slot if there are not enough in the pool
                var newSlot = Instantiate(_inventorySlotTemplate, _inventorySlotsParent);
                newSlot.Bind(inventoryViewData.ContainerPath, inventorySlot.ToInt64());
                _slots.Add(newSlot);
                newSlot.gameObject.SetActive(true);
                newSlot.Render(stack);
            }

            // Update text fields and drop areas visibility
            _pneumaText.text = inventoryViewData.ViewPneuma ? $"Pneuma: {inventoryViewData.Pneuma}" : "";
            _goldText.text = inventoryViewData.ViewGold ? $"Gold: {inventoryViewData.Gold}" : "";
            _slotsAmountText.text = inventoryViewData.ViewSlotsAmount ? $"Slots: {inventoryViewData.SlotsUsed} / {inventoryViewData.SlotsMax}" : "";
            _weightText.text = inventoryViewData.ViewWeight ? $"Weight: {inventoryViewData.WeightCurrent} / {inventoryViewData.WeightMax}" : "";

            _enchantDropArea.gameObject.SetActive(inventoryViewData.ViewEnchantDropArea);
            _destroyDropArea.gameObject.SetActive(inventoryViewData.ViewDestroyDropArea);
        }
    }
}
