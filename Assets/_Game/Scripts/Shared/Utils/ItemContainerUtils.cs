using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.Items.Traits;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.Shared.Utils
{
    // todo: convert to non-static class; encapsulate rules
    public static class ItemContainerUtils
    {
        /// <summary>
        /// Move between containers without specifying destination slot. Target container chooses placement.
        /// Returns actually moved amount.
        /// </summary>
        public static int MoveAmount<TFromSlot>(
            IItemContainer<TFromSlot> from,
            TFromSlot fromSlot,
            IItemContainer to,
            int amount)
            where TFromSlot : notnull, IContainerSlot
        {
            if (amount <= 0) return 0;

            var fromSnapshotNullable = from.Get(fromSlot);
            if (fromSnapshotNullable == null) return 0;
            var fromSnapshot = fromSnapshotNullable.Value;
            if (fromSnapshot.Amount <= 0) return 0;

            int take = Math.Min(amount, fromSnapshot.Amount);

            // Phase 1: remove from source
            int removed = from.RemoveFromSlot(fromSlot, take);
            if (removed <= 0) return 0;

            // Phase 2: add to target
            int added = to.Add(new(fromSnapshot.Definition, fromSnapshot.InstanceData, removed), AddPolicy.StackThenEmpty);

            // Rollback remainder if target couldn't accept all
            int remainder = removed - added;
            if (remainder > 0)
            {
                // Best-effort rollback: try stack-then-empty back into source (not necessarily same slot)
                int rolledBack = from.Add(new(fromSnapshot.Definition, fromSnapshot.InstanceData, remainder), AddPolicy.StackThenEmpty);

                // If rollback also couldn't fit (shouldn't happen unless capacity changed),
                // we "lose" items - in production you'd drop to world or keep an overflow buffer.
                // Here we enforce correctness:
                if (rolledBack != remainder)
                    throw new InvalidOperationException($"Rollback failed. Expected {remainder}, rolled back {rolledBack}.");
            }

            return added;
        }

        /// <summary>
        /// Move amount from specific slot to specific slot. If toSlot is null, target container chooses placement.
        /// Returns actually moved amount.
        /// </summary>
        public static int MoveAmount(
            IItemContainer from,
            long fromSlot,
            IItemContainer to,
            long toSlot,
            int amount)
        {
            if (amount <= 0) return 0;

            var fromSnapshotNullable = from.Get(fromSlot);
            if (fromSnapshotNullable == null) return 0;
            var fromSnapshot = fromSnapshotNullable.Value;
            if (fromSnapshot.Amount <= 0) return 0;

            int take = Math.Min(amount, fromSnapshot.Amount);

            // Phase 1: remove from source
            int removed = from.RemoveFromSlot(fromSlot, take);
            if (removed <= 0) return 0;

            // Phase 2: add to target
            int added;
            if (toSlot < 0)
            {
                added = to.Add(new(fromSnapshot.Definition, fromSnapshot.InstanceData, removed), AddPolicy.StackThenEmpty);
            }
            else
            {
                added = to.AddToSlot(toSlot, new(fromSnapshot.Definition, fromSnapshot.InstanceData, removed));
            }

            // Rollback remainder if target couldn't accept all
            int remainder = removed - added;
            if (remainder > 0)
            {
                // Best-effort rollback: try stack-then-empty back into source (not necessarily same slot)
                int rolledBack = from.Add(new(fromSnapshot.Definition, fromSnapshot.InstanceData, remainder), AddPolicy.StackThenEmpty);

                // If rollback also couldn't fit (shouldn't happen unless capacity changed),
                // we "lose" items - in production you'd drop to world or keep an overflow buffer.
                // Here we enforce correctness:
                if (rolledBack != remainder)
                    throw new InvalidOperationException($"Rollback failed. Expected {remainder}, rolled back {rolledBack}.");
            }

            return added;
        }

        /// <summary>Move within same container (drag-drop), optional destination slot.</summary>
        public static int MoveAmount(
            IItemContainer container,
            long fromSlot,
            long toSlot,
            int amount)
        {
            return MoveAmount(container, fromSlot, container, toSlot, amount);
        }

        public static bool TryMoveAmountWithRollback(
            IItemContainer fromContainer,
            long fromSlot,
            IItemContainer toContainer,
            long toSlot,
            int amount)
        {
            int moved = MoveAmount(fromContainer, fromSlot, toContainer, toSlot, amount);
            if (moved == amount)
                return true;

            if (moved > 0 && MoveAmount(toContainer, toSlot, fromContainer, fromSlot, moved) != moved)
                throw new InvalidOperationException("Rollback failed after a partial stack move.");

            return false;
        }

        public static bool TrySwapBetweenContainerSlots(
            IItemContainer fromContainer,
            long fromSlot,
            IItemContainer toContainer,
            long toSlot)
        {
            if (fromContainer == null ||
                toContainer == null ||
                fromSlot < 0 ||
                toSlot < 0 ||
                (ReferenceEquals(fromContainer, toContainer) && fromSlot == toSlot))
                return false;

            var fromSnapshot = fromContainer.Get(fromSlot);
            var toSnapshot = toContainer.Get(toSlot);

            if (fromSnapshot == null && toSnapshot == null)
                return false;

            if (fromSnapshot != null &&
                toSnapshot != null &&
                fromSnapshot.Value.Key.Equals(toSnapshot.Value.Key) &&
                fromSnapshot.Value.Amount <= toSnapshot.Value.Definition.MaxAmount - toSnapshot.Value.Amount)
            {
                return TryMoveAmountWithRollback(fromContainer, fromSlot, toContainer, toSlot, fromSnapshot.Value.Amount);
            }

            if (ReferenceEquals(fromContainer, toContainer) && fromContainer is InventoryModel inventoryModel)
                return inventoryModel.Swap(InventorySlot.FromInt64(fromSlot), InventorySlot.FromInt64(toSlot));

            if (fromSnapshot == null)
            {
                return TryMoveAmountWithRollback(toContainer, toSlot, fromContainer, fromSlot, toSnapshot.Value.Amount);
            }

            if (toSnapshot == null)
            {
                return TryMoveAmountWithRollback(fromContainer, fromSlot, toContainer, toSlot, fromSnapshot.Value.Amount);
            }

            int removedFrom = fromContainer.RemoveFromSlot(fromSlot, fromSnapshot.Value.Amount);
            if (removedFrom != fromSnapshot.Value.Amount)
            {
                if (removedFrom > 0)
                    fromContainer.AddToSlot(fromSlot, new(fromSnapshot.Value.Definition, fromSnapshot.Value.InstanceData, removedFrom));
                return false;
            }

            int removedTo = toContainer.RemoveFromSlot(toSlot, toSnapshot.Value.Amount);
            if (removedTo != toSnapshot.Value.Amount)
            {
                // We got here because we successfully removed from the first container so returning all
                fromContainer.AddToSlot(fromSlot, fromSnapshot.Value);
                if (removedTo > 0)
                    toContainer.AddToSlot(toSlot, new(toSnapshot.Value.Definition, toSnapshot.Value.InstanceData, removedTo));
                return false;
            }

            int addedTo = toContainer.AddToSlot(toSlot, fromSnapshot.Value);
            int addedFrom = fromContainer.AddToSlot(fromSlot, toSnapshot.Value);
            if (addedTo == fromSnapshot.Value.Amount && addedFrom == toSnapshot.Value.Amount)
                return true;

            if (addedTo > 0)
                toContainer.RemoveFromSlot(toSlot, addedTo);
            if (addedFrom > 0)
                fromContainer.RemoveFromSlot(fromSlot, addedFrom);
            fromContainer.AddToSlot(fromSlot, fromSnapshot.Value);
            toContainer.AddToSlot(toSlot, toSnapshot.Value);
            return false;
        }

        /// <summary>
        /// Returns actually removed amount.
        /// </summary>
        public static int RemoveAmount<TSlot>(
            IItemContainer<TSlot> container,
            TSlot fromSlot,
            int amount)
            where TSlot : notnull, IContainerSlot
        {
            if (container.Get(fromSlot) == null) return 0;
            return container.RemoveFromSlot(fromSlot, amount);
        }

        public static bool Has(this IItemContainer container, ItemKey key, int amount)
        {
            if (amount <= 0) return true;
            return container.Count(key) >= amount;
        }

        public static bool TryEquipWithSwap(
            IItemContainer<InventorySlot> fromContainer,
            InventorySlot fromSlot,
            EquipmentSlotKey equipmentSlot,
            EquipmentModel equipmentModel,
            ItemStackSnapshot fromItem)
        {
            var secondarySlotKeys = GetSecondarySlotKeys(fromItem, equipmentModel);

            // Fast-path: try to equip directly if secondary slots are free (no swap needed)
            var secondarySlotsAvailable = secondarySlotKeys.All(slot => equipmentModel.Get(slot) == null);
            if (secondarySlotsAvailable)
            {
                // 1) Fast path: try to equip directly into the requested equipment slot (no swap).
                if (equipmentModel.PreviewAddToSlot(equipmentSlot, fromItem) == fromItem.Amount)
                {
                    if (fromContainer.RemoveFromSlot(fromSlot, fromItem.Amount) == 0)
                        return false;

                    int equipped = equipmentModel.AddToSlot(equipmentSlot, fromItem);
                    if (equipped == 0)
                    {
                        fromContainer.Add(fromItem, AddPolicy.StackThenEmpty);
                        return false;
                    }

                    ClearSlots(equipmentModel, secondarySlotKeys);
                    return true;
                }

                // 2) Fast path: try to equip without specifying the equipment slot (no swap).
                if (equipmentModel.PreviewAdd(fromItem) >= 1)
                {
                    if (fromContainer.RemoveFromSlot(fromSlot, fromItem.Amount) == 0)
                        return false;

                    int equipped = equipmentModel.Add(fromItem);
                    if (equipped == 0)
                    {
                        fromContainer.Add(fromItem, AddPolicy.StackThenEmpty);
                        return false;
                    }

                    ClearSlots(equipmentModel, secondarySlotKeys);
                    return true;
                }
            }

            var oldItemNullable = equipmentModel.Get(equipmentSlot);
            if (oldItemNullable == null)
                return false;

            var oldItem = oldItemNullable.Value;
            if (oldItem.Amount <= 0)
                return false;

            // Collect items from secondary slots that the new item will occupy
            var secondarySlotItems = new List<(EquipmentSlotKey slot, ItemStackSnapshot item)>();
            foreach (var slot in secondarySlotKeys)
            {
                var secondaryItem = equipmentModel.Get(slot);
                if (secondaryItem != null)
                    secondarySlotItems.Add((slot, secondaryItem.Value));
            }

            // Check if we have enough space to store all items before starting the swap
            if (!CanStoreAllItems(fromContainer, oldItem, secondarySlotItems))
                return false;

            // Remove old equipment items
            if (!RemoveOldEquipment(equipmentModel, equipmentSlot, oldItem, secondarySlotItems, out var removedSecondaryItems))
                return false;

            // Remove new item from source container
            if (fromContainer.RemoveFromSlot(fromSlot, fromItem.Amount) == 0)
            {
                RollbackRemoveOldEquipment(equipmentModel, equipmentSlot, oldItem, removedSecondaryItems);
                return false;
            }

            // Equip new item
            int equippedNew = equipmentModel.AddToSlot(equipmentSlot, fromItem);
            if (equippedNew == 0)
            {
                fromContainer.Add(fromItem, AddPolicy.StackThenEmpty);
                RollbackRemoveOldEquipment(equipmentModel, equipmentSlot, oldItem, removedSecondaryItems);
                return false;
            }

            // Clear secondary slots for the new item
            ClearSlots(equipmentModel, secondarySlotKeys);

            // Store old items in container
            int storedOld = fromContainer.Add(oldItem, AddPolicy.StackThenEmpty);
            if (storedOld != oldItem.Amount)
            {
                // This should not happen if CanStoreAllItems check was correct
                // Rollback the entire operation
                equipmentModel.RemoveFromSlot(equipmentSlot, fromItem.Amount);
                fromContainer.Add(fromItem, AddPolicy.StackThenEmpty);
                RollbackRemoveOldEquipment(equipmentModel, equipmentSlot, oldItem, removedSecondaryItems);
                SLog.Error($"Failed to store old equipped item back to container during equip with swap. Expected to store {oldItem.Amount}, but only stored {storedOld}. Rolling back equip operation.");
                return false;
            }

            // Store secondary slot items in container
            foreach (var (_, item) in secondarySlotItems)
            {
                int stored = fromContainer.Add(item, AddPolicy.StackThenEmpty);
                if (stored != item.Amount)
                {
                    SLog.Error($"Failed to store item from secondary slot back to container during equip with swap. Expected to store {item.Amount}, but only stored {stored}. This may lead to item loss. Please investigate.");
                    // This should not happen if CanStoreAllItems check was correct
                    // At this point we've already partially completed the operation
                    // We can't fully rollback, so this is a critical error
                    // The best we can do is try to store what we can
                    return false;
                }
            }

            return true;
        }

        private static List<EquipmentSlotKey> GetSecondarySlotKeys(ItemStackSnapshot item, EquipmentModel equipmentModel)
        {
            var secondarySlotKeys = new List<EquipmentSlotKey>();
            if (!item.Definition.TryGetTrait<EquippableTrait>(out var equippableTrait) ||
                equippableTrait.SecondarySlots == null ||
                equippableTrait.SecondarySlots.Length == 0)
                return secondarySlotKeys;

            foreach (var secondarySlotType in equippableTrait.SecondarySlots)
            {
                var slotKey = equipmentModel.Slots.FirstOrDefault(s => s.SlotType == secondarySlotType);
                if (equipmentModel.IsValidSlot(slotKey))
                    secondarySlotKeys.Add(slotKey);
            }

            return secondarySlotKeys;
        }

        private static void ClearSlots(EquipmentModel equipmentModel, List<EquipmentSlotKey> slotKeys)
        {
            foreach (var slot in slotKeys)
            {
                equipmentModel.RemoveFromSlot(slot, int.MaxValue);
            }
        }

        private static bool CanStoreAllItems(IItemContainer<InventorySlot> container, ItemStackSnapshot oldItem, List<(EquipmentSlotKey slot, ItemStackSnapshot item)> secondarySlotItems)
        {
            // Conservative check: ensure there are enough empty slots for all items
            // Count total items that need to be stored
            int totalItemsToStore = 1 + secondarySlotItems.Count;

            // Count empty slots in the container
            int emptySlots = 0;
            if (container is InventoryModel inventoryModel)
            {
                foreach (var (_, snapshot) in inventoryModel.Enumerate())
                {
                    if (snapshot == null)
                        emptySlots++;
                }

                // We need at least as many empty slots as items to store
                // This is conservative but safe for equipment items (which don't stack)
                return emptySlots >= totalItemsToStore;
            }

            // Fallback for non-inventory containers: check each item individually
            if (container.PreviewAdd(oldItem, AddPolicy.StackThenEmpty) < oldItem.Amount)
                return false;

            foreach (var (_, item) in secondarySlotItems)
            {
                if (container.PreviewAdd(item, AddPolicy.StackThenEmpty) < item.Amount)
                    return false;
            }

            return true;
        }

        private static bool RemoveOldEquipment(
            EquipmentModel equipmentModel,
            EquipmentSlotKey equipmentSlot,
            ItemStackSnapshot oldItem,
            List<(EquipmentSlotKey slot, ItemStackSnapshot item)> secondarySlotItems,
            out List<(EquipmentSlotKey slot, ItemStackSnapshot item, int removed)> removedSecondaryItems)
        {
            removedSecondaryItems = new List<(EquipmentSlotKey slot, ItemStackSnapshot item, int removed)>();

            int removedOld = equipmentModel.RemoveFromSlot(equipmentSlot, oldItem.Amount);
            if (removedOld != oldItem.Amount)
                return false;

            foreach (var (slot, item) in secondarySlotItems)
            {
                int removed = equipmentModel.RemoveFromSlot(slot, item.Amount);
                if (removed != item.Amount)
                {
                    equipmentModel.AddToSlot(equipmentSlot, oldItem);
                    foreach (var (prevSlot, prevItem, _) in removedSecondaryItems)
                    {
                        equipmentModel.AddToSlot(prevSlot, prevItem);
                    }
                    return false;
                }
                removedSecondaryItems.Add((slot, item, removed));
            }

            return true;
        }

        private static void RollbackRemoveOldEquipment(
            EquipmentModel equipmentModel,
            EquipmentSlotKey equipmentSlot,
            ItemStackSnapshot oldItem,
            List<(EquipmentSlotKey slot, ItemStackSnapshot item, int removed)> removedSecondaryItems)
        {
            equipmentModel.AddToSlot(equipmentSlot, oldItem);
            foreach (var (slot, item, _) in removedSecondaryItems)
            {
                equipmentModel.AddToSlot(slot, item);
            }
        }
    }
}
