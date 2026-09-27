using Assets._Game.Scripts.Items.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Views.Widgets
{
    public sealed class ShopSlotWidget : MonoBehaviour
    {
        [SerializeField]
        private Image _icon;
        [SerializeField]
        private TMP_Text _amountText;
        [SerializeField]
        private TMP_Text _priceText;

        private ShopSlot _slot;

        public void Bind(ShopSlot slot)
        {
            _slot = slot;
        }

        public void Render(ShopSlotViewData data)
        {
            if (!data.HasItem)
            {
                _icon.enabled = false;
                _amountText.text = string.Empty;
                _priceText.text = string.Empty;
                return;
            }

            _icon.enabled = true;
            _icon.sprite = data.Icon;
            _amountText.text = data.AmountText;
            _priceText.text = data.PriceText;
        }
    }

    public readonly struct ShopSlotViewData
    {
        public ShopSlot Slot { get; }
        public bool HasItem { get; }
        public Sprite Icon { get; }
        public string AmountText { get; }
        public string PriceText { get; }

        public ShopSlotViewData(ShopSlot slot, bool hasItem, Sprite icon, string amountText, string priceText)
        {
            Slot = slot;
            HasItem = hasItem;
            Icon = icon;
            AmountText = amountText;
            PriceText = priceText;
        }
    }
}
