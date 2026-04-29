using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Traits;
using System;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Shared.Extensions
{
    public static class ItemStackExtensions
    {
        public static T GetFunctionalTrait<T>(this ItemStackSnapshot itemStack, ItemTrigger trigger) where T : FunctionalItemTraitBase
        {
            if (itemStack.Definition.TryGetTrait<T>(out var trait) && trait.Triggers.HasFlag(trigger))
            {
                return trait;
            }
            return null;
        }

        public static T GetTrait<T>(this ItemStackSnapshot itemStack) where T : ItemTraitBase
        {
            return itemStack.Definition.GetTrait<T>();
        }

        public static IEnumerable<T> GetFunctionalTraits<T>(this ItemStackSnapshot itemStack, ItemTrigger trigger) where T : FunctionalItemTraitBase
        {
            var traits = itemStack.Definition.GetTraits<T>();
            foreach (var trait in traits)
            {
                if (trait.Triggers.HasFlag(trigger))
                {
                    yield return trait;
                }
            }
        }

        public static IEnumerable<T> GetTraits<T>(this ItemStackSnapshot itemStack) where T : ItemTraitBase
        {
            return itemStack.Definition.GetTraits<T>();
        }

        public static EquippableTrait GetEquippableTrait(this ItemStackSnapshot itemStack)
        {
            return itemStack.Definition.GetTrait<EquippableTrait>();
        }

        public static EquipmentSlotType GetEquipmentSlotType(this ItemStackSnapshot itemStack)
        {
            var equippableTrait = GetEquippableTrait(itemStack);
            return equippableTrait != null ? equippableTrait.Slot : EquipmentSlotType.None;
        }

        public static bool IsEquippable(this ItemStackSnapshot itemStack)
        {
            return GetEquipmentSlotType(itemStack) != EquipmentSlotType.None;
        }

        public static bool HasId(this ItemStackSnapshot item, string id)
        {
            return string.Equals(item.Definition.Id, id, StringComparison.OrdinalIgnoreCase);
        }

        public static ItemStackPurpose GetPurpose(this ItemStackSnapshot itemStack)
        {
            if (itemStack.IsEquippable())
            {
                var slotType = itemStack.GetEquipmentSlotType();
                return slotType switch
                {
                    EquipmentSlotType.Weapon => ItemStackPurpose.Weapon,
                    EquipmentSlotType.OffHand => ItemStackPurpose.Weapon,
                    EquipmentSlotType.Armor => ItemStackPurpose.Clothing,
                    EquipmentSlotType.Helmet => ItemStackPurpose.Clothing,
                    EquipmentSlotType.Gloves => ItemStackPurpose.Clothing,
                    EquipmentSlotType.Boots => ItemStackPurpose.Clothing,
                    EquipmentSlotType.Ring => ItemStackPurpose.Clothing,
                    EquipmentSlotType.Necklace => ItemStackPurpose.Clothing,
                    EquipmentSlotType.Utility => ItemStackPurpose.Utility,
                    _ => ItemStackPurpose.None
                };
            }
            else if (itemStack.Definition.Traits.Length == 0)
            {
                return ItemStackPurpose.Resource;
            }
            return ItemStackPurpose.None;
        }
    }

    public enum ItemStackPurpose
    {
        None,
        Weapon = 10,
        Clothing = 20,
        Utility = 30,
        Resource = 40
    }
}
