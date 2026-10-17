using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views.Widgets;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class ItemEnchantingView : UIViewBase<ItemEnchantingViewData>
    {
        [SerializeField]
        private ItemStackWidget _enchantableItemWidget;
        [SerializeField]
        private ItemStackWidget _enchantingItemWidget;
        [SerializeField]
        private Button _enchantButton;
        [SerializeField]
        private Button _cancelButton;

        protected override void Render(ItemEnchantingViewData data)
        {
            _enchantableItemWidget.Bind(data.EnchantableItemPath);
            _enchantableItemWidget.Render(data.EnchantableItem);

            _enchantingItemWidget.Bind(data.EnchantingItemPath);
            _enchantingItemWidget.Render(data.EnchantingItem);
        }
    }
}
