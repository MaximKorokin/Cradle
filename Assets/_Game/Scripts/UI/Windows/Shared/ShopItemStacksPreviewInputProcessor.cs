using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.Items.Shop;
using Assets._Game.Scripts.UI.Services;

namespace Assets._Game.Scripts.UI.Windows.Shared
{
    /// <summary>
    /// Input processor specifically for shop windows that handles buy/sell operations.
    /// </summary>
    public sealed class ShopItemStacksPreviewInputProcessor
    {
        private readonly ItemPreviewService _itemPreviewService;
        private readonly InventoryModel _inventoryModel;
        private readonly ShopModel _shopModel;
        private readonly float _buyCoefficient;
        private readonly float _sellCoefficient;

        public ShopItemStacksPreviewInputProcessor(
            ItemPreviewService itemPreviewService,
            InventoryModel inventoryModel,
            ShopModel shopModel,
            float buyCoefficient,
            float sellCoefficient)
        {
            _itemPreviewService = itemPreviewService;
            _inventoryModel = inventoryModel;
            _shopModel = shopModel;
            _buyCoefficient = buyCoefficient;
            _sellCoefficient = sellCoefficient;
        }

        public void OnInventorySlotClick(InventorySlot slot)
        {
            var item = _inventoryModel.Get(slot);
            if (item == null) return;

            _itemPreviewService.ShowInventoryItemForSellPreview(
                slot.ToInt64(),
                _shopModel,
                _sellCoefficient);
        }

        public void OnShopSlotClick(ShopSlot slot)
        {
            var item = _shopModel.Get(slot);
            if (item == null) return;

            _itemPreviewService.ShowShopItemPreview(
                slot.ToInt64(),
                _shopModel,
                _buyCoefficient);
        }
    }
}
