using Assets._Game.Scripts.UI.Views.Widgets;
using Assets._Game.Scripts.Items.Shop;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class ShopView : UIViewBase<(IReadOnlyList<ShopSlotViewData> ShopSlots, string ShopName, float BuyCoefficient, float SellCoefficient)>
    {
        [SerializeField]
        private RectTransform _shopSlotsParent;
        [SerializeField]
        private ShopSlotWidget _shopSlotTemplate;
        [Space]
        [SerializeField]
        private TMP_Text _shopNameText;
        [SerializeField]
        private TMP_Text _buyCoefficientText;
        [SerializeField]
        private TMP_Text _sellCoefficientText;

        private readonly List<ShopSlotWidget> _slots = new();

        public event Action<ShopSlot> SlotClicked;

        public override void Render((IReadOnlyList<ShopSlotViewData> ShopSlots, string ShopName, float BuyCoefficient, float SellCoefficient) data)
        {
            _shopSlotTemplate.gameObject.SetActive(false);

            _shopNameText.text = data.ShopName;
            _buyCoefficientText.text = $"Buy: {data.BuyCoefficient:0.#}x";
            _sellCoefficientText.text = $"Sell: {data.SellCoefficient:0.#}x";

            foreach (var slot in _slots)
            {
                slot.gameObject.SetActive(false);
            }

            for (int i = 0; i < data.ShopSlots.Count; i++)
            {
                var shopSlot = data.ShopSlots[i];
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
