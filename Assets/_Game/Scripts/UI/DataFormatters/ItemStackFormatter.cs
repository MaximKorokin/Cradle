using Assets._Game.Scripts.Items;
using UnityEngine;

namespace Assets._Game.Scripts.UI.DataFormatters
{
    public sealed class ItemStackFormatter : IDataFormatter<ItemStackSnapshot, ItemStackDisplayData>
    {
        private readonly ItemDefinitionFormatter _itemDefinitionFormatter;

        public ItemStackFormatter(ItemDefinitionFormatter itemDefinitionFormatter)
        {
            _itemDefinitionFormatter = itemDefinitionFormatter;
        }

        public ItemStackDisplayData FormatData(ItemStackSnapshot data)
        {
            var definitionData = _itemDefinitionFormatter.FormatData(data.Definition);

            var amount = data.Definition.MaxAmount > 1 ? $"Amount: {data.Amount}" : string.Empty;
            var weight = string.Empty;

            if (data.Definition.Weight == 0)
            {
                weight = string.Empty;
            }
            else if (data.Definition.MaxAmount > 1 && data.Amount > 1)
            {
                weight = $"Weight: {data.Definition.Weight * data.Amount} ({data.Definition.Weight} each)";
            }
            else
            {
                weight = $"Weight: {data.Definition.Weight}";
            }

            return new ItemStackDisplayData(
                definitionData.Name,
                definitionData.Icon,
                amount,
                weight,
                definitionData.PriceText,
                definitionData.IsEquippable,
                definitionData.EquipmentSlotName,
                definitionData.EquippableEffectsText,
                definitionData.IsUsable,
                definitionData.IsConsumable,
                definitionData.UsableCooldownText,
                definitionData.UsableEffectsText,
                definitionData.Description);
        }
    }

    public readonly struct ItemStackDisplayData
    {
        public bool HasData { get; }

        public string Name { get; }
        public Sprite Icon { get; }
        public string AmountText { get; }
        public string WeightText { get; }
        public string PriceText { get; }

        public bool IsEquippable { get; }
        public string EquipmentSlotName { get; }
        public string EquippableEffectsText { get; }

        public bool IsUsable { get; }
        public bool IsConsumable { get; }
        public string UsableCooldownText { get; }
        public string UsableEffectsText { get; }

        public string Description { get; }

        public ItemStackDisplayData(
            string name,
            Sprite icon,
            string amount,
            string weight,
            string price,
            bool isEquippable,
            string equipmentSlotName,
            string equippableEffectsText,
            bool isUsable,
            bool isConsumable,
            string usableCooldownText,
            string usableEffectsText,
            string description = null)
        {
            HasData = true;

            Name = name;
            Icon = icon;
            AmountText = amount;
            WeightText = weight;
            PriceText = price;
            IsEquippable = isEquippable;
            EquipmentSlotName = equipmentSlotName;
            EquippableEffectsText = equippableEffectsText;
            IsUsable = isUsable;
            IsConsumable = isConsumable;
            UsableCooldownText = usableCooldownText;
            UsableEffectsText = usableEffectsText;
            Description = description;
        }
    }
}
