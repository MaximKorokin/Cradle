using Assets._Game.Scripts.UI.Views.Widgets;
using System.Collections.Generic;
using Assets._Game.Scripts.UI.DataAggregators;
using TMPro;
using UnityEngine;
using Assets._Game.Scripts.UI.Systems.DragDrop;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class ShopView : UIViewBase<ShopViewData>, IDragDropTarget
    {
        [SerializeField]
        private Image _highlightImage;
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

        protected override void Render(ShopViewData data)
        {
            _shopSlotTemplate.gameObject.SetActive(false);

            _shopNameText.text = data.ShopName;
            _buyCoefficientText.text = $"Buy: {data.BuyCoefficient:0.#}x";
            _sellCoefficientText.text = $"Sell: {data.SellCoefficient:0.#}x";

            foreach (var slot in _slots)
            {
                slot.gameObject.SetActive(false);
            }

            foreach (var (shopSlot, item, isInfinite, priceText) in data.Enumerate())
            {
                if (_slots.Count > shopSlot.Index)
                {
                    var slot = _slots[shopSlot.Index];
                    slot.Bind(this, data.ContainerPath, shopSlot.ToInt64());
                    slot.Render(item, isInfinite, priceText);
                    slot.gameObject.SetActive(true);
                    continue;
                }

                var newSlot = Instantiate(_shopSlotTemplate, _shopSlotsParent);
                newSlot.Bind(this, data.ContainerPath, shopSlot.ToInt64());
                _slots.Add(newSlot);
                newSlot.gameObject.SetActive(true);
                newSlot.Render(item, isInfinite, priceText);
            }
        }

        public void SetDragDropHighlight(bool highlighted)
        {
            _highlightImage.enabled = highlighted;
        }
    }
}
