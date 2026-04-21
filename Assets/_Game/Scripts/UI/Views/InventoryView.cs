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
        private TMP_Text _slotsAmountText;
        [Space]
        [SerializeField]
        private Button _filterByArmorButton;
        [SerializeField]
        private Button _filterByWeaponButton;
        [SerializeField]
        private Button _filterByConsumableButton;
        [SerializeField]
        private Button _filterByResourceButton;
        [Space]
        [SerializeField]
        private Button _OrderByNameButton;
        [SerializeField]
        private Button _OrderByTypeButton;

        private readonly List<InventorySlotView> _slots = new();

        private IInventoryHudData _inventoryHudData;

        public event Action<InventorySlot> SlotClick;

        public event Action FilterByArmorButtonClicked;
        public event Action FilterByWeaponButtonClicked;
        public event Action FilterByConsumableButtonClicked;
        public event Action FilterByResourceButtonClicked;

        public event Action OrderByNameButtonClicked;
        public event Action OrderByTypeButtonClicked;

        private void OnEnable()
        {
            _filterByArmorButton.onClick.AddListener(() => FilterByArmorButtonClicked?.Invoke());
            _filterByWeaponButton.onClick.AddListener(() => FilterByWeaponButtonClicked?.Invoke());
            _filterByConsumableButton.onClick.AddListener(() => FilterByConsumableButtonClicked?.Invoke());
            _filterByResourceButton.onClick.AddListener(() => FilterByResourceButtonClicked?.Invoke());

            _OrderByNameButton.onClick.AddListener(() => OrderByNameButtonClicked?.Invoke());
            _OrderByTypeButton.onClick.AddListener(() => OrderByTypeButtonClicked?.Invoke());
        }

        private void OnDisable()
        {
            _filterByArmorButton.onClick.RemoveAllListeners();
            _filterByWeaponButton.onClick.RemoveAllListeners();
            _filterByConsumableButton.onClick.RemoveAllListeners();
            _filterByResourceButton.onClick.RemoveAllListeners();

            _OrderByNameButton.onClick.RemoveAllListeners();
            _OrderByTypeButton.onClick.RemoveAllListeners();
        }

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

            foreach (var (inventorySlot, stack) in _inventoryHudData.InventoryModel.Enumerate())
            {
                if (_slots.Count > inventorySlot.Index)
                {
                    var slot = _slots[inventorySlot.Index];
                    slot.Render(stack);
                    slot.gameObject.SetActive(true);
                    continue;
                }

                var slotView = Instantiate(_inventorySlotTemplate, _inventorySlotsParent);
                slotView.Bind(inventorySlot);
                slotView.PointerClick += OnSlotPointerClick;
                _slots.Add(slotView);
                slotView.gameObject.SetActive(true);
                slotView.Render(stack);
            }

            _weightText.text = _inventoryHudData.ViewWeight ? $"{_inventoryHudData.WeightCurrent}kg / {_inventoryHudData.WeightMax}kg" : "";
            _goldText.text = _inventoryHudData.ViewGold ? $"{_inventoryHudData.Gold} gold" : "";
            _slotsAmountText.text = _inventoryHudData.ViewSlotsAmount ? $"{_inventoryHudData.SlotsUsed} / {_inventoryHudData.SlotsMax} slots" : "";
        }

        public void Unbind()
        {
            if (_inventoryHudData != null)
            {
                _inventoryHudData.Changed -= OnInventoryHudDataChanged;
                _inventoryHudData = null;
            }
        }

        private void OnSlotPointerClick(InventorySlot slotIndex)
        {
            SlotClick?.Invoke(slotIndex);
        }
    }
}
