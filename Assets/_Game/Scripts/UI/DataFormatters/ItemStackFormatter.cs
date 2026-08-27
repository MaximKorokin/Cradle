using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;
using UnityEngine;

namespace Assets._Game.Scripts.UI.DataFormatters
{
    public sealed class ItemStackFormatter : IDataFormatter<(ItemStackSnapshot, EquipmentModel), ItemStackDisplayData>
    {
        private readonly ItemDefinitionFormatter _itemDefinitionFormatter;
        private readonly ItemSetFormatter _itemSetFormatter;

        public ItemStackFormatter(
            ItemDefinitionFormatter itemDefinitionFormatter,
            ItemSetFormatter itemSetFormatter)
        {
            _itemDefinitionFormatter = itemDefinitionFormatter;
            _itemSetFormatter = itemSetFormatter;
        }

        public ItemStackDisplayData FormatData((ItemStackSnapshot, EquipmentModel) data)
        {
            var (itemStackSnapshot, equipmentModel) = data;

            var definitionData = _itemDefinitionFormatter.FormatData(itemStackSnapshot.Definition);

            var amount = itemStackSnapshot.Definition.MaxAmount > 1 ? $"Amount: {itemStackSnapshot.Amount}" : string.Empty;

            string weight;
            if (itemStackSnapshot.Definition.Weight == 0)
            {
                weight = string.Empty;
            }
            else if (itemStackSnapshot.Definition.MaxAmount > 1 && itemStackSnapshot.Amount > 1)
            {
                weight = $"Weight: {itemStackSnapshot.Definition.Weight * itemStackSnapshot.Amount} ({itemStackSnapshot.Definition.Weight} each)";
            }
            else
            {
                weight = $"Weight: {itemStackSnapshot.Definition.Weight}";
            }

            var itemSetDisplayData = equipmentModel != null ? _itemSetFormatter.FormatData((itemStackSnapshot.Definition, equipmentModel)) : definitionData.ItemSetDisplayData;

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
                itemSetDisplayData,
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

        public ItemSetDisplayData ItemSetDisplayData { get; }

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
            ItemSetDisplayData itemSetDisplayData,
            string description)
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
            ItemSetDisplayData = itemSetDisplayData;
            Description = description;
        }
    }
}
