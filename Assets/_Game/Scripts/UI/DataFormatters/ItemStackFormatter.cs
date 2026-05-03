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
                definitionData.PriceDescription,
                definitionData.IsEquippable,
                definitionData.EquipmentSlotName,
                definitionData.EquippableEffectsDescription,
                definitionData.IsUsable,
                definitionData.IsConsumable,
                definitionData.UsableCooldownDescription,
                definitionData.UsableEffectsDescription);
        }
    }

    public readonly struct ItemStackDisplayData
    {
        public bool HasData { get; }

        public string Name { get; }
        public Sprite Icon { get; }
        public string AmountDescription { get; }
        public string WeightDescription { get; }
        public string PriceDescription { get; }

        public bool IsEquippable { get; }
        public string EquipmentSlotName { get; }
        public string EquippableEffectsDescription { get; }

        public bool IsUsable { get; }
        public bool IsConsumable { get; }
        public string UsableCooldownDescription { get; }
        public string UsableEffectsDescription { get; }

        public ItemStackDisplayData(
            string name,
            Sprite icon,
            string amount,
            string weight,
            string price,
            bool isEquippable,
            string equipmentSlotName,
            string equippableEffectsDescription,
            bool isUsable,
            bool isConsumable,
            string usableCooldownDescription,
            string usableEffectsDescription)
        {
            HasData = true;

            Name = name;
            Icon = icon;
            AmountDescription = amount;
            WeightDescription = weight;
            PriceDescription = price;
            IsEquippable = isEquippable;
            EquipmentSlotName = equipmentSlotName;
            EquippableEffectsDescription = equippableEffectsDescription;
            IsUsable = isUsable;
            IsConsumable = isConsumable;
            UsableCooldownDescription = usableCooldownDescription;
            UsableEffectsDescription = usableEffectsDescription;
        }
    }
}
