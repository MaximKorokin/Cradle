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

            var isEquippable = false;
            var equipmentSlotName = string.Empty;
            var equippableEffectsDescription = string.Empty;

            var isUsable = false;
            var isConsumable = false;
            var usableCooldownDescription = string.Empty;
            var usableEffectsDescription = string.Empty;

            if (definition.TryGetTrait<EquippableTrait>(out var equippableTrait))
            {
                isEquippable = true;
                equipmentSlotName = equippableTrait.Slot.ToString();
                equippableEffectsDescription = GetFunctionalTraitsDescription(definition, ItemTrigger.OnEquipmentChange);
            }

            if (definition.TryGetTrait<UsableTrait>(out var usableTrait))
            {
                isUsable = true;
                isConsumable = usableTrait.Consumable;
                usableCooldownDescription = $"{usableTrait.Cooldown}s";
                usableEffectsDescription = GetFunctionalTraitsDescription(definition, ItemTrigger.OnUse);
            }

            return new ItemStackDisplayData(
                name,
                icon,
                amount,
                weight,
                isEquippable,
                equipmentSlotName,
                equippableEffectsDescription,
                isUsable,
                isConsumable,
                usableCooldownDescription,
                usableEffectsDescription);
        }

        private string GetFunctionalTraitsDescription(ItemDefinition definition, ItemTrigger itemTrigger)
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
