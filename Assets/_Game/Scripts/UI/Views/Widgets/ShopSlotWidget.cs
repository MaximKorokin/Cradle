using Assets._Game.Scripts.Items;
using TMPro;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views.Widgets
{
    public sealed class ShopSlotWidget : ContainerSlotWidget
    {
        [SerializeField]
        private TMP_Text _amountText;
        [SerializeField]
        private TMP_Text _priceText;

        public ShopView ShopView { get; private set; }

        public void Bind(ShopView shopView, ItemContainerPath containerPath, long slotIndex)
        {
            ShopView = shopView;
            base.Bind(containerPath, slotIndex);
        }

        public void Render(ItemStackSnapshot? itemStack, bool isInfinite, string priceText)
        {
            base.Render(itemStack, Color.white);
            if (!ContainsData)
            {
                _amountText.text = string.Empty;
                _priceText.text = string.Empty;
                return;
            }

            _amountText.text = isInfinite ? string.Empty : itemStack.Value.Amount.ToString();
            _priceText.text = priceText;
        }
    }
}
