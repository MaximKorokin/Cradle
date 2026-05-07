using Assets._Game.Scripts.Items.Shop;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class ShopView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _shopSlotsParent;
        [SerializeField]
        private ShopSlotView _shopSlotTemplate;
        [Space]
        [SerializeField]
        private TMP_Text _shopNameText;
        [SerializeField]
        private TMP_Text _buyCoefficientText;
        [SerializeField]
        private TMP_Text _sellCoefficientText;

        private readonly List<ShopSlotView> _slots = new();

        public event Action<ShopSlot> SlotClicked;

        public void Render(IReadOnlyList<ShopSlotViewData> shopSlots, string shopName, float buyCoefficient, float sellCoefficient)
        {
            _shopSlotTemplate.gameObject.SetActive(false);

            _shopNameText.text = shopName;
            _buyCoefficientText.text = $"Buy: {buyCoefficient:0.#}x";
            _sellCoefficientText.text = $"Sell: {sellCoefficient:0.#}x";

            foreach (var slot in _slots)
            {
                slot.gameObject.SetActive(false);
            }

            for (int i = 0; i < shopSlots.Count; i++)
            {
                var shopSlot = shopSlots[i];
                if (_slots.Count > i)
                {
                    var slot = _slots[i];
                    slot.Render(shopSlot);
                    slot.gameObject.SetActive(true);
                    continue;
                }

                var newSlot = Instantiate(_shopSlotTemplate, _shopSlotsParent);
                newSlot.Bind(shopSlot.Slot);
                newSlot.PointerClick += OnSlotPointerClick;
                _slots.Add(newSlot);
                newSlot.gameObject.SetActive(true);
                newSlot.Render(shopSlot);
            }
        }

        private void OnSlotPointerClick(ShopSlot slotIndex)
        {
            SlotClicked?.Invoke(slotIndex);
        }
    }
}
