using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Traits;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.Shared.Extensions
{
    public static class ItemStackExtensions
    {
        public static IEnumerable<T> GetTraits<T>(this ItemStackSnapshot itemStack)
        {
            return itemStack.Definition.Traits.Where(t => t is T).Cast<T>();
        }

        public static EquippableTrait GetEquippableTrait(this ItemStackSnapshot itemStack)
        {
            return itemStack.Definition.Traits.FirstOrDefault(x => x is EquippableTrait trait) as EquippableTrait;
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
    }
}
