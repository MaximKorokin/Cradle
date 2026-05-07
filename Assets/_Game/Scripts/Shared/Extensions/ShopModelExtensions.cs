using Assets._Game.Scripts.Items.Shop;
using Assets._Game.Scripts.Items.Traits;
using System;

namespace Assets._Game.Scripts.Shared.Extensions
{
    public static class ShopModelExtensions
    {
        public static bool TryGetBuyPrice(this ShopModel shop, ShopSlot shopSlot, float buyCoefficient, float sellCoefficient, out int buyPrice)
        {
            buyPrice = 0;
            var itemStack = shop.Get(shopSlot);
            if (!itemStack.HasValue) return false;

            if (!itemStack.Value.Definition.TryGetTrait<PriceTrait>(out var priceTrait)) return false;

            // Finite item means that it was sold and can be bought back
            var coefficient = shop.IsInfinite(shopSlot) ? buyCoefficient : sellCoefficient;

            var price = (int)(priceTrait.BasePrice * coefficient);
            buyPrice = Math.Max(price, 1);
            return true;
        }
    }
}
