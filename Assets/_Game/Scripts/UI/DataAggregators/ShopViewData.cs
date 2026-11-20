using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Shop;
using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.Shared.Extensions;
using System.Collections.Generic;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public sealed class ShopViewData : ItemContainerDataAggregatorBase
    {
        public string ShopName { get; private set; }
        public float BuyCoefficient { get; private set; }
        public float SellCoefficient { get; private set; }

        private IReadOnlyObservableData<EntryRef> _buyerEntityId;

        public ItemContainerPath BuyerInventoryPath => ItemContainerPath.Inventory(_buyerEntityId?.Value.Id);

        private ShopModel ShopModel => ItemContainer as ShopModel;

        public ShopViewData(
            ItemContainerResolver itemContainerResolver,
            EntityRepository entityRepository) : base(itemContainerResolver, entityRepository) { }

        public void SetShopData(
            string shopName,
            float buyCoefficient,
            float sellCoefficient,
            IReadOnlyObservableData<EntryRef> buyerEntityId)
        {
            ShopName = shopName;
            BuyCoefficient = buyCoefficient;
            SellCoefficient = sellCoefficient;
            _buyerEntityId = buyerEntityId;
        }

        public IEnumerable<(ShopSlot Slot, ItemStackSnapshot? Item, bool IsInfinite, string PriceText)> Enumerate()
        {
            foreach (var (slot, snapshot) in ShopModel.Enumerate())
            {
                var price = snapshot.HasValue && ShopModel.TryGetBuyPrice(slot, BuyCoefficient, SellCoefficient, out var buyPrice)
                    ? buyPrice
                    : 0;
                yield return (slot, snapshot, ShopModel.IsInfinite(slot), FormatPriceText(price));
            }
        }

        private string FormatPriceText(int price)
        {
            if (price == 0) return string.Empty;
            if (price > 1_000) return $"{price / 1_000.0:F1}k";
            if (price > 1_000_000) return $"{price / 1_000_000.0:F1}M";
            if (price > 1_000_000_000) return $"{price / 1_000_000_000.0:F1}B";
            return price.ToString();
        }

        protected override ItemContainerPath GetContainerPath(string entityId) => ItemContainerPath.Shop(entityId);
    }
}
