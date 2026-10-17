using Assets._Game.Scripts.Items;
using TMPro;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views.Widgets
{
    public sealed class ShopSlotWidget : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _priceText;
        [SerializeField]
        private ItemStackWidget _itemStackWidget;

        public ShopView ShopView { get; private set; }

        public ItemStackWidget SlotView => _itemStackWidget;

        public void Bind(ShopView shopView, ItemContainerPath containerPath, long slotIndex)
        {
            ShopView = shopView;
            _itemStackWidget.Bind(containerPath, slotIndex);
        }

        public void Render(ItemStackSnapshot? itemStack, bool isInfinite, string priceText)
        {
            _itemStackWidget.Render(itemStack, Color.white);
            if (!_itemStackWidget.ContainsData)
            {
                _priceText.text = string.Empty;
                return;
            }

            _priceText.text = priceText;
        }
    }
}
