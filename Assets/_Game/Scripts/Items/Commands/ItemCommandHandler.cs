using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.Shared.Utils;
using System;

namespace Assets._Game.Scripts.Items.Commands
{
    public class ItemCommandHandler
    {
        public bool Handle<T>(IItemCommand cmd)
        {
            return cmd switch
            {
                MoveItemCommand<T> c => HandleMove(c),
                EquipFromContainerCommand<T> c => HandleEquip(c),
                UnequipToContainerCommand c => HandleUnequip(c),
                DropItemCommand<T> c => HandleDrop(c),
                _ => throw new NotSupportedException(cmd.GetType().Name),
            };
        }

        private bool HandleMove<T>(MoveItemCommand<T> c)
        {
            return ItemContainerUtils.MoveAmount(c.FromContainer, c.FromSlot, c.ToContainer, c.Amount) > 0;
        }

        private bool HandleEquip<T>(EquipFromContainerCommand<T> c) where T : notnull
        {
            // Read the source slot snapshot.
            var fromItemNullable = c.FromContainer.Get(c.FromSlot);
            if (fromItemNullable == null)
                return false;

            var fromItem = fromItemNullable.Value;
            if (fromItem.Amount <= 0)
                return false;

            // --- 1) Fast path: try to equip directly into the requested equipment slot (no swap).
            if (c.EquipmentModel.PreviewAddToSlot(c.EquipmentSlot, fromItem) == fromItem.Amount)
            {
                // Remove from source first.
                if (c.FromContainer.RemoveFromSlot(c.FromSlot, fromItem.Amount) == 0)
                    return false;

                // Put into equipment slot.
                int equipped = c.EquipmentModel.AddToSlot(c.EquipmentSlot, fromItem);
                if (equipped == 0)
                {
                    // Rollback: return the item back to the source container.
                    c.FromContainer.Add(fromItem, AddPolicy.StackThenEmpty);
                    return false;
                }

                return true;
            }

            // --- 2) Fast path: try to equip without specifying the equipment slot (no swap).
            if (c.EquipmentModel.PreviewAdd(fromItem) >= 1)
            {
                // Remove from source first.
                if (c.FromContainer.RemoveFromSlot(c.FromSlot, fromItem.Amount) == 0)
                    return false;

                // Put into equipment slot.
                int equipped = c.EquipmentModel.Add(fromItem);
                if (equipped == 0)
                {
                    // Rollback: return the item back to the source container.
                    c.FromContainer.Add(fromItem, AddPolicy.StackThenEmpty);
                    return false;
                }

                return true;
            }

            // --- 3) Swap path: the target equipment slot is not directly acceptive (occupied or incompatible).
            // We only perform swap if there is an item currently equipped in that slot.
            var oldItemNullable = c.EquipmentModel.Get(c.EquipmentSlot);
            if (oldItemNullable == null)
                return false;

            var oldItem = oldItemNullable.Value;
            if (oldItem.Amount <= 0)
                return false;

            // Before we unequip anything, ensure the old equipped item can be stored back in the source container.
            // This prevents "losing" items if the source inventory is full.
            if (c.FromContainer.PreviewAdd(oldItem, AddPolicy.StackThenEmpty) < oldItem.Amount)
                return false;

            // Unequip the old item (clear the equipment slot).
            int removedOld = c.EquipmentModel.RemoveFromSlot(c.EquipmentSlot, oldItem.Amount);
            if (removedOld != oldItem.Amount)
                return false;

            // Remove the new item from the source.
            if (c.FromContainer.RemoveFromSlot(c.FromSlot, fromItem.Amount) == 0)
            {
                // Rollback: re-equip the old item.
                c.EquipmentModel.AddToSlot(c.EquipmentSlot, oldItem);
                return false;
            }

            // Equip the new item into the now-empty equipment slot.
            int equippedNew = c.EquipmentModel.AddToSlot(c.EquipmentSlot, fromItem);
            if (equippedNew == 0)
            {
                // Rollback: return the new item to the source and re-equip the old item.
                c.FromContainer.Add(fromItem, AddPolicy.StackThenEmpty);
                c.EquipmentModel.AddToSlot(c.EquipmentSlot, oldItem);
                return false;
            }

            // Store the previously equipped item back into the source container.
            int storedOld = c.FromContainer.Add(oldItem, AddPolicy.StackThenEmpty);
            if (storedOld != oldItem.Amount)
            {
                // This should not happen because of the PreviewAdd check above,
                // but if it does, perform a hard rollback to keep the state consistent.
                c.EquipmentModel.RemoveFromSlot(c.EquipmentSlot, fromItem.Amount);
                c.FromContainer.Add(fromItem, AddPolicy.StackThenEmpty);
                c.EquipmentModel.AddToSlot(c.EquipmentSlot, oldItem);
                throw new InvalidOperationException("Swap failed: cannot store old equipped item in source container.");
            }

            return true;
        }

        private bool HandleUnequip(UnequipToContainerCommand c)
        {
            var item = c.EquipmentModel.Get(c.SlotKey);
            if (item == null) return false;
            return ItemContainerUtils.MoveAmount(c.EquipmentModel, c.SlotKey, c.ToContainer, item.Value.Amount) > 0;
        }

        private bool HandleDrop<T>(DropItemCommand<T> c)
        {
            return ItemContainerUtils.RemoveAmount(c.FromContainer, c.FromSlot, c.Amount) > 0;
        }
    }
}
