using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.Shared.Extensions;
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
                int rolledBack = from.AddToSlot(fromSlot, new(fromSnapshot.Definition, fromSnapshot.InstanceData, remainder));

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
                int rolledBack = from.AddToSlot(fromSlot, new(fromSnapshot.Definition, fromSnapshot.InstanceData, remainder));

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
        public static int RemoveAmount(
            IItemContainer container,
            long fromSlot,
            int amount)
        {
            if (container.Get(fromSlot) == null) return 0;
            return container.RemoveFromSlot(fromSlot, amount);
        }

        public static bool Has(this IItemContainer container, ItemKey key, int amount)
        {
            if (amount <= 0) return true;
            return container.Count(key) >= amount;
        }

        public static int MoveToEquipmentSlot(
            IItemContainer<InventorySlot> fromContainer,
            InventorySlot fromSlot,
            EquipmentModel equipmentModel,
            EquipmentSlotKey? requestedEquipmentSlot,
            int? amount)
        {
            var sourceItem = fromContainer.Get(fromSlot);
            if (sourceItem == null || !sourceItem.Value.IsEquippable())
                return 0;

            int transferAmount = Math.Min(amount ?? sourceItem.Value.Amount, sourceItem.Value.Amount);
            if (transferAmount <= 0)
                return 0;

            var transferItem = new ItemStackSnapshot(sourceItem.Value.Definition, sourceItem.Value.InstanceData, transferAmount);
            var equipmentSlot = requestedEquipmentSlot ?? default;
            if (!requestedEquipmentSlot.HasValue)
            {
                int addableAmount = equipmentModel.PreviewAdd(transferItem);
                if (addableAmount > 0)
                    return MoveAmount(fromContainer, fromSlot, equipmentModel, addableAmount);

                equipmentSlot = equipmentModel.Slots.FirstOrDefault(slot => slot.SlotType == transferItem.GetEquipmentSlotType());
            }

            if (equipmentSlot.SlotType == EquipmentSlotType.None || !equipmentModel.IsValidSlot(equipmentSlot))
                return 0;

            int addableToSlot = equipmentModel.PreviewAddToSlot(equipmentSlot, transferItem);
            if (addableToSlot > 0)
                return MoveAmount(fromContainer, fromSlot.ToInt64(), equipmentModel, equipmentSlot.ToInt64(), addableToSlot);

            return TryEquipWithSwap(fromContainer, fromSlot, equipmentSlot, equipmentModel, transferItem)
                ? transferAmount
                : 0;
        }

        public static bool TryEquipWithSwap(
            IItemContainer<InventorySlot> fromContainer,
            InventorySlot fromSlot,
            EquipmentSlotKey equipmentSlot,
            EquipmentModel equipmentModel,
            ItemStackSnapshot fromItem,
            bool removeOnlyDirectConflicts = false)
        {
            if (fromItem.GetEquipmentSlotType() != equipmentSlot.SlotType)
                return false;

            // Fast-path: try to equip directly if the rules allow it (no swap needed)
            if (equipmentModel.PreviewAddToSlot(equipmentSlot, fromItem) == fromItem.Amount)
            {
                if (fromContainer.RemoveFromSlot(fromSlot, fromItem.Amount) == 0)
                    return false;

                int equipped = equipmentModel.AddToSlot(equipmentSlot, fromItem);
                if (equipped == 0)
                {
                    fromContainer.AddToSlot(fromSlot, fromItem);
                    return false;
                }

                return true;
            }

            var conflictSlots = new HashSet<EquipmentSlotKey>(equipmentModel.GetConflictingSlots(equipmentSlot, fromItem));
            if (!removeOnlyDirectConflicts)
            {
                var pendingSlots = new Queue<EquipmentSlotKey>(conflictSlots);
                while (pendingSlots.Count > 0)
                {
                    var slot = pendingSlots.Dequeue();
                    var item = equipmentModel.Get(slot);
                    if (item == null) continue;

                    foreach (var conflictSlot in equipmentModel.GetConflictingSlots(slot, item.Value))
                        if (conflictSlots.Add(conflictSlot))
                            pendingSlots.Enqueue(conflictSlot);
                }
            }

            var conflictingItems = new List<(EquipmentSlotKey slot, ItemStackSnapshot item)>();
            foreach (var slot in equipmentModel.Slots)
            {
                if (!conflictSlots.Contains(slot)) continue;
                var item = equipmentModel.Get(slot);
                if (item != null)
                    conflictingItems.Add((slot, item.Value));
            }

            if (conflictingItems.Count == 0)
                return false;

            EquipmentSlotKey? sourceSlotItem = conflictingItems
                .Where(x => x.slot.Equals(equipmentSlot))
                .Select(x => (EquipmentSlotKey?)x.slot)
                .FirstOrDefault();
            sourceSlotItem ??= conflictingItems
                .Where(x => equipmentModel.GetSecondarySlots(x.item).Contains(equipmentSlot))
                .Select(x => (EquipmentSlotKey?)x.slot)
                .FirstOrDefault();
            sourceSlotItem ??= conflictingItems[0].slot;

            var sourceItem = fromContainer.Get(fromSlot);
            int additionalEmptySlots = sourceItem != null && sourceItem.Value.Amount == fromItem.Amount ? 1 : 0;
            if (!CanStoreAllItems(fromContainer, conflictingItems, additionalEmptySlots))
                return false;

            if (!RemoveEquipment(equipmentModel, conflictingItems, out var removedItems))
                return false;

            if (equipmentModel.PreviewAddToSlot(equipmentSlot, fromItem) != fromItem.Amount)
            {
                RollbackRemoveEquipment(equipmentModel, removedItems);
                return false;
            }

            // Remove new item from source container
            if (fromContainer.RemoveFromSlot(fromSlot, fromItem.Amount) == 0)
            {
                RollbackRemoveEquipment(equipmentModel, removedItems);
                return false;
            }

            // Equip new item
            int equippedNew = equipmentModel.AddToSlot(equipmentSlot, fromItem);
            if (equippedNew == 0)
            {
                fromContainer.AddToSlot(fromSlot, fromItem);
                RollbackRemoveEquipment(equipmentModel, removedItems);
                return false;
            }

            // Store displaced items back to source container
            foreach (var (slot, item) in conflictingItems.OrderBy(x => x.slot.Equals(sourceSlotItem.Value) ? 0 : 1))
            {
                int stored = slot.Equals(sourceSlotItem.Value)
                    ? fromContainer.AddToSlot(fromSlot, item)
                    : fromContainer.Add(item, AddPolicy.StackThenEmpty);
                int remainder = item.Amount - stored;
                if (remainder > 0)
                {
                    int storedElsewhere = fromContainer.Add(
                        new(item.Definition, item.InstanceData, remainder),
                        AddPolicy.StackThenEmpty);
                    if (storedElsewhere != remainder)
                    {
                        SLog.Error($"Failed to store displaced equipment item back to container during equip. Expected to store {remainder}, but only stored {storedElsewhere}. This may lead to item loss. Please investigate.");
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool CanStoreAllItems(
            IItemContainer<InventorySlot> container,
            List<(EquipmentSlotKey slot, ItemStackSnapshot item)> items,
            int additionalEmptySlots = 0)
        {
            // Conservative check: ensure there are enough empty slots for all items
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
                return emptySlots + additionalEmptySlots >= items.Count;
            }

            // Fallback for non-inventory containers: check each item individually
            foreach (var (_, item) in items)
            {
                if (container.PreviewAdd(item, AddPolicy.StackThenEmpty) < item.Amount)
                    return false;
            }

            return true;
        }

        private static bool RemoveEquipment(
            EquipmentModel equipmentModel,
            List<(EquipmentSlotKey slot, ItemStackSnapshot item)> items,
            out List<(EquipmentSlotKey slot, ItemStackSnapshot item)> removedItems)
        {
            removedItems = new List<(EquipmentSlotKey slot, ItemStackSnapshot item)>();

            foreach (var (slot, item) in items)
            {
                int removed = equipmentModel.RemoveFromSlot(slot, item.Amount);
                if (removed != item.Amount)
                {
                    if (removed > 0)
                        equipmentModel.AddToSlot(slot, new(item.Definition, item.InstanceData, removed));
                    RollbackRemoveEquipment(equipmentModel, removedItems);
                    return false;
                }
                removedItems.Add((slot, item));
            }

            return true;
        }

        private static void RollbackRemoveEquipment(
            EquipmentModel equipmentModel,
            List<(EquipmentSlotKey slot, ItemStackSnapshot item)> removedItems)
        {
            foreach (var (slot, item) in removedItems)
            {
                equipmentModel.AddToSlot(slot, item);
            }
        }
    }
}
