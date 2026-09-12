using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Traits;
using System;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Shared.Extensions
{
    public static class ItemDefinitionExtensions
    {
        public static IEnumerable<T> GetFunctionalTraits<T>(this ItemDefinition definition, ItemTrigger trigger) where T : FunctionalItemTraitBase
        {
            var traits = definition.GetTraits<T>();
            foreach (var trait in traits)
            {
                if (trait.Triggers.HasFlag(trigger))
                {
                    yield return trait;
                }
            }
        }

        public static bool TryGetSellPrice(this ItemDefinition itemDefinition, float sellCoefficient, out int sellPrice)
        {
            sellPrice = 0;
            if (!itemDefinition.TryGetTrait<PriceTrait>(out var priceTrait)) return false;

            var price = (int)(priceTrait.BasePrice * sellCoefficient);
            sellPrice = Math.Max(price, 1);
            return true;
        }
    }
}
