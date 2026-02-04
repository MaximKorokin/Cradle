using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Traits;
using Assets.CoreScripts;
using System;
using System.Linq;

namespace Assets._Game.Scripts.Shared.Extensions
{
    public static class ItemStackExtensions
    {
        public static bool CanAddTo(this ItemStack source, ItemStack target)
        {
            return source.Definition.Id == target.Definition.Id
                && source.Instance is IImmutableInstanceData sourceImmutableInstanceData
                && target.Instance is IImmutableInstanceData targetImmutableInstanceData
                && sourceImmutableInstanceData.GetStackKey() == targetImmutableInstanceData.GetStackKey()
                && target.Amount < target.Definition.MaxAmount;
        }

        public static bool CanAddToCompletely(this ItemStack source, ItemStack target)
        {
            return CanAddTo(source, target) && source.Amount + target.Amount <= target.Definition.MaxAmount;
        }

        public static void AddTo(this ItemStack source, ItemStack target)
        {
            var remaining = target.Definition.MaxAmount - target.Amount;
            // No space left
            if (remaining == 0) return;

            // Can't stack
            if (!CanAddTo(source, target))
            {
                SLog.Error($"Tried to add incompatible ItemStacks: {source.Definition.Id} and {target.Definition.Id}");
                return;
            }

            // Can fit all
            if (source.Amount <= remaining)
            {
                target.Amount += source.Amount;
                source.Amount = 0;
            }
            // Can fit some
            else
            {
                target.Amount += remaining;
                source.Amount -= remaining;
            }
        }

        public static void RemoveFrom(this ItemStack target, ref int amount)
        {
            if (amount <= 0) return;

            var toRemove = Math.Min(target.Amount, amount);
            target.Amount -= toRemove;
            amount -= toRemove;
        }

        public static bool IsEquippable(this ItemStack itemStack)
        {
            return GetEquipmentSlotType(itemStack) != EquipmentSlotType.None;
        }

        public static EquipmentSlotType GetEquipmentSlotType(this ItemStack itemStack)
        {
            var equippableTrait = itemStack.Definition.Traits.FirstOrDefault(x => x is EquippableTrait trait) as EquippableTrait;
            return equippableTrait != null ? equippableTrait.Slot : EquipmentSlotType.None;
        }

        public static bool HasId(this ItemStack item, string id)
        {
            return string.Equals(item.Definition.Id, id, StringComparison.OrdinalIgnoreCase);
        }
    }
}
