using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;
using UnityEngine;

namespace Assets._Game.Scripts.UI.DataFormatters
{
    public sealed class ItemStackFormatter : IDataFormatter<(ItemStackSnapshot, EquipmentModel), ItemStackDisplayData>
    {
        private readonly ItemDefinitionFormatter _itemDefinitionFormatter;
        private readonly ItemSetFormatter _itemSetFormatter;
        private readonly EnchantableTraitFormatter _enchantableTraitFormatter;

        public ItemStackFormatter(
            ItemDefinitionFormatter itemDefinitionFormatter,
            ItemSetFormatter itemSetFormatter,
            EnchantableTraitFormatter enchantableTraitFormatter)
        {
            _itemDefinitionFormatter = itemDefinitionFormatter;
            _itemSetFormatter = itemSetFormatter;
            _enchantableTraitFormatter = enchantableTraitFormatter;
        }

        public ItemStackDisplayData FormatData((ItemStackSnapshot, EquipmentModel) data)
        {
            var (itemStackSnapshot, equipmentModel) = data;

            var definitionData = _itemDefinitionFormatter.FormatData(itemStackSnapshot.Definition);

            var prefix = GetPrefixText(itemStackSnapshot);

            var amount = itemStackSnapshot.Definition.MaxAmount > 1 ? $"Amount: {itemStackSnapshot.Amount}" : string.Empty;

            var weight = GetWeightText(itemStackSnapshot);

            var enchantableDisplayData = _enchantableTraitFormatter.FormatData((itemStackSnapshot.Definition, itemStackSnapshot.InstanceData));
            var itemSetDisplayData = equipmentModel != null ? _itemSetFormatter.FormatData((itemStackSnapshot.Definition, equipmentModel)) : definitionData.ItemSetDisplayData;

            return new ItemStackDisplayData(
                prefix,
                definitionData.Name,
                definitionData.Icon,
                amount,
                weight,
                definitionData.PriceText,
                definitionData.IsEquippable,
                definitionData.EquipmentSlotName,
                definitionData.EquippableEffectsText,
                definitionData.HasInInventoryEffects,
                definitionData.InInventoryEffectsText,
                definitionData.IsUsable,
                definitionData.IsConsumable,
                definitionData.UsableCooldownText,
                definitionData.UsableEffectsText,
                enchantableDisplayData,
                itemSetDisplayData,
                definitionData.Description);
        }

        private string GetPrefixText(ItemStackSnapshot itemStackSnapshot)
        {
            if (itemStackSnapshot.InstanceData == null)
            {
                return string.Empty;
            }
            if (itemStackSnapshot.InstanceData.TryGet<EnchantInstanceData>(out var enchantInstanceData) && enchantInstanceData.Level > 0)
            {
                return $"+{enchantInstanceData.Level} ";
            }
            return string.Empty;
        }

        private string GetWeightText(ItemStackSnapshot itemStackSnapshot)
        {
            if (itemStackSnapshot.Definition.Weight == 0)
            {
                return string.Empty;
            }
            else if (itemStackSnapshot.Definition.MaxAmount > 1 && itemStackSnapshot.Amount > 1)
            {
                return $"Weight: {itemStackSnapshot.Definition.Weight * itemStackSnapshot.Amount} ({itemStackSnapshot.Definition.Weight} each)";
            }
            else
            {
                return $"Weight: {itemStackSnapshot.Definition.Weight}";
            }
        }
    }

    public readonly struct ItemStackDisplayData
    {
        public bool HasData { get; }

        public string Name { get; }
        public Sprite Icon { get; }
        public string PrefixText { get; }
        public string AmountText { get; }
        public string WeightText { get; }
        public string PriceText { get; }

        public bool IsEquippable { get; }
        public string EquipmentSlotName { get; }
        public string EquippableEffectsText { get; }

        public bool HasInInventoryEffects { get; }
        public string InInventoryEffectsText { get; }

        public bool IsUsable { get; }
        public bool IsConsumable { get; }
        public string UsableCooldownText { get; }
        public string UsableEffectsText { get; }

        public EnchantDisplayData EnchantableDisplayData { get; }
        public ItemSetDisplayData ItemSetDisplayData { get; }

        public string Description { get; }

        public ItemStackDisplayData(
            string prefixText,
            string name,
            Sprite icon,
            string amount,
            string weight,
            string price,
            bool isEquippable,
            string equipmentSlotName,
            string equippableEffectsText,
            bool hasInInventoryEffects,
            string inInventoryEffectsText,
            bool isUsable,
            bool isConsumable,
            string usableCooldownText,
            string usableEffectsText,
            EnchantDisplayData enchantableDisplayData,
            ItemSetDisplayData itemSetDisplayData,
            string description)
        {
            HasData = true;

            PrefixText = prefixText;
            Name = name;
            Icon = icon;
            AmountText = amount;
            WeightText = weight;
            PriceText = price;
            IsEquippable = isEquippable;
            EquipmentSlotName = equipmentSlotName;
            EquippableEffectsText = equippableEffectsText;
            HasInInventoryEffects = hasInInventoryEffects;
            InInventoryEffectsText = inInventoryEffectsText;
            IsUsable = isUsable;
            IsConsumable = isConsumable;
            UsableCooldownText = usableCooldownText;
            UsableEffectsText = usableEffectsText;
            EnchantableDisplayData = enchantableDisplayData;
            ItemSetDisplayData = itemSetDisplayData;
            Description = description;
        }
    }
}
