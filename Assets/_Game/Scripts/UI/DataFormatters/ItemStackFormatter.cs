using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Traits;
using Assets._Game.Scripts.Shared.Extensions;
using System;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.UI.DataFormatters
{
    public sealed class ItemStackFormatter : IDataFormatter<ItemStackSnapshot, ItemStackDisplayData>
    {
        private readonly FunctionalItemTraitFormatter _functionalItemTraitFormatter;

        public ItemStackFormatter(FunctionalItemTraitFormatter functionalItemTraitFormatter)
        {
            _functionalItemTraitFormatter = functionalItemTraitFormatter;
        }

        public ItemStackDisplayData FormatData(ItemStackSnapshot data)
        {
            var name = data.Definition.Name;
            var icon = data.Definition.Icon;
            var amount = data.Definition.MaxAmount > 1 ? $"Amount: {data.Amount}" : string.Empty;
            var weight = string.Empty;

            var isEquippable = false;
            var equipmentSlotName = string.Empty;
            var equippableEffectsDescription = string.Empty;

            var isUsable = false;
            var isConsumable = false;
            var usableCooldownDescription = string.Empty;
            var usableEffectsDescription = string.Empty;

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

            if (data.Definition.TryGetTrait<EquippableTrait>(out var equippableTrait))
            {
                isEquippable = true;
                equipmentSlotName = equippableTrait.Slot.ToString();
                equippableEffectsDescription = GetFunctionalTraitsDescription(data, ItemTrigger.OnEquipmentChange);
            }

            if (data.Definition.TryGetTrait<UsableTrait>(out var usableTrait))
            {
                isUsable = true;
                isConsumable = usableTrait.Consumable;
                usableCooldownDescription = $"{usableTrait.Cooldown}s";
                usableEffectsDescription = GetFunctionalTraitsDescription(data, ItemTrigger.OnUse);
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

        private string GetFunctionalTraitsDescription(ItemStackSnapshot itemStack, ItemTrigger itemTrigger)
        {
            var traits = itemStack.GetFunctionalTraits<FunctionalItemTraitBase>(itemTrigger).ToArray();
            if (traits.Length == 0) return string.Empty;

            return string.Join($"{Environment.NewLine}{Environment.NewLine}", traits.Select(effect => _functionalItemTraitFormatter.FormatData(effect)));
        }
    }

    public readonly struct ItemStackDisplayData
    {
        public bool HasData { get; }

        public string Name { get; }
        public Sprite Icon { get; }
        public string AmountDescription { get; }
        public string WeightDescription { get; }

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
