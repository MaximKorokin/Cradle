using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Traits;
using System;
using System.Linq;

namespace Assets._Game.Scripts.UI.DataFormatters
{
    public sealed class ItemDefinitionFormatter : IDataFormatter<ItemDefinition, ItemStackDisplayData>
    {
        private readonly FunctionalItemTraitFormatter _functionalItemTraitFormatter;

        public ItemDefinitionFormatter(FunctionalItemTraitFormatter functionalItemTraitFormatter)
        {
            _functionalItemTraitFormatter = functionalItemTraitFormatter;
        }

        public ItemStackDisplayData FormatData(ItemDefinition definition)
        {
            var name = definition.Name;
            var icon = definition.Icon;
            var amount = string.Empty;
            var weight = definition.Weight == 0 ? string.Empty : $"Weight: {definition.Weight}";
            var price = string.Empty;

            if (definition.TryGetTrait<PriceTrait>(out var priceTrait))
            {
                price = $"Base Price: {priceTrait.BasePrice}g";
            }

            var isEquippable = false;
            var equipmentSlotName = string.Empty;
            var equippableEffectsText = string.Empty;

            var isUsable = false;
            var isConsumable = false;
            var usableCooldownText = string.Empty;
            var usableEffectsText = string.Empty;

            if (definition.TryGetTrait<EquippableTrait>(out var equippableTrait))
            {
                isEquippable = true;
                equipmentSlotName = equippableTrait.Slot.ToString();
                equippableEffectsText = GetFunctionalTraitsText(definition, ItemTrigger.OnEquipmentChange);
            }

            if (definition.TryGetTrait<UsableTrait>(out var usableTrait))
            {
                isUsable = true;
                isConsumable = usableTrait.Consumable;
                usableCooldownText = $"{usableTrait.Cooldown}s";
                usableEffectsText = GetFunctionalTraitsText(definition, ItemTrigger.OnUse);
            }

            var description = string.Empty;
            if (definition.TryGetTrait<DescriptionTrait>(out var descriptionTrait))
            {
                description = descriptionTrait.Description;
            }

            return new ItemStackDisplayData(
                name,
                icon,
                amount,
                weight,
                price,
                isEquippable,
                equipmentSlotName,
                equippableEffectsText,
                isUsable,
                isConsumable,
                usableCooldownText,
                usableEffectsText,
                description);
        }

        private string GetFunctionalTraitsText(ItemDefinition definition, ItemTrigger itemTrigger)
        {
            var traits = definition.GetTraits<FunctionalItemTraitBase>()
                .Where(t => t.Triggers.HasFlag(itemTrigger))
                .ToArray();

            if (traits.Length == 0) return string.Empty;

            return string.Join(
                $"{Environment.NewLine}{Environment.NewLine}",
                traits
                    .Select(trait => _functionalItemTraitFormatter.FormatData(trait))
                    .Where(text => !string.IsNullOrEmpty(text)));
        }
    }
}
