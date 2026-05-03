using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.Items.Traits;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.Items.Commands
{
    public class ItemCommandHandler
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly ItemContainerResolver _itemContainerResolver;

        public ItemCommandHandler(IGlobalEventBus globalEventBus, ItemContainerResolver itemContainerResolver)
        {
            _globalEventBus = globalEventBus;
            _itemContainerResolver = itemContainerResolver;
        }

        public bool Handle(Entity entity, IItemCommand command)
        {
            return command switch
            {
                TransferItemCommand c => HandleTransfer(entity, c),
                EquipFromContainerCommand c => HandleEquip(entity, c),
                UnequipToContainerCommand c => HandleUnequip(entity, c),
                DropItemCommand c => HandleDrop(entity, c),
                UseItemCommand c => HandleUse(entity, c),
                _ => throw new NotSupportedException(command.GetType().Name),
            };
        }

        private ItemUseSettings ResolveItemUseSettings(Entity entity, bool isManual)
        {
            var equipmentModule = entity.GetModule<EquipmentModule>();
            return isManual ? equipmentModule.ManualItemUseSettings : equipmentModule.AutoItemUseSettings;
        }

        private List<EquipmentSlotKey> GetSecondarySlotKeys(ItemStackSnapshot item, EquipmentModel equipmentModel)
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

        private List<(EquipmentSlotKey slot, ItemStackSnapshot item)> CollectItemsFromSlots(EquipmentModel equipmentModel, List<EquipmentSlotKey> slots)
        {
            var items = new List<(EquipmentSlotKey slot, ItemStackSnapshot item)>();
            foreach (var slot in slots)
            {
                var item = equipmentModel.Get(slot);
                if (item != null)
                    items.Add((slot, item.Value));
            }
            return items;
        }

        private bool HandleTransfer(Entity entity, TransferItemCommand c)
        {
            var from = _itemContainerResolver.ResolveInventory(entity, c.FromContainer);
            var to = _itemContainerResolver.ResolveContainer(entity, c.ToContainer);
            return ItemContainerUtils.MoveAmount(from, ContainerSlotConverter.ToInventorySlot(c.FromSlot), to, c.Amount) > 0;
        }

        private bool HandleEquip(Entity entity, EquipFromContainerCommand c)
        {
            var fromContainer = _itemContainerResolver.ResolveInventory(entity, c.FromContainer);
            var fromSlot = ContainerSlotConverter.ToInventorySlot(c.FromSlot);
            var equipmentSlot = ContainerSlotConverter.ToEquipmentSlot(c.EquipmentSlot);
            var equipmentModel = _itemContainerResolver.ResolveEquipment(entity);

            var fromItemNullable = fromContainer.Get(fromSlot);
            if (fromItemNullable == null)
                return false;

            var fromItem = fromItemNullable.Value;
            if (fromItem.Amount <= 0)
                return false;

            var secondarySlotKeys = GetSecondarySlotKeys(fromItem, equipmentModel);
            // All secondary slots are empty or there are no secondary slots
            var secondarySlotsAvailable = secondarySlotKeys.All(slot => equipmentModel.Get(slot) == null);

            if (TryEquipDirectly(fromContainer, fromSlot, equipmentSlot, equipmentModel, fromItem, secondarySlotKeys, secondarySlotsAvailable))
                return true;

            return HandleEquipWithSwap(fromContainer, fromSlot, equipmentSlot, equipmentModel, fromItem, secondarySlotKeys);
        }

        private bool TryEquipDirectly(
            IItemContainer<InventorySlot> fromContainer,
            InventorySlot fromSlot,
            EquipmentSlotKey equipmentSlot,
            EquipmentModel equipmentModel,
            ItemStackSnapshot fromItem,
            List<EquipmentSlotKey> secondarySlotKeys,
            bool secondarySlotsAvailable)
        {
            if (!secondarySlotsAvailable)
                return false;

            // --- 1) Fast path: try to equip directly into the requested equipment slot (no swap).
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

                ClearSecondarySlots(equipmentModel, secondarySlotKeys);
                return true;
            }

            // --- 2) Fast path: try to equip without specifying the equipment slot (no swap).
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

                ClearSecondarySlots(equipmentModel, secondarySlotKeys);
                return true;
            }

            return false;
        }

        private bool HandleEquipWithSwap(
            IItemContainer<InventorySlot> fromContainer,
            InventorySlot fromSlot,
            EquipmentSlotKey equipmentSlot,
            EquipmentModel equipmentModel,
            ItemStackSnapshot fromItem,
            List<EquipmentSlotKey> secondarySlotKeys)
        {
            var oldItemNullable = equipmentModel.Get(equipmentSlot);
            if (oldItemNullable == null)
                return false;

            var oldItem = oldItemNullable.Value;
            if (oldItem.Amount <= 0)
                return false;

            // Collect items from secondary slots that the new item will occupy
            var secondarySlotItems = CollectItemsFromSlots(equipmentModel, secondarySlotKeys);

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
            ClearSecondarySlots(equipmentModel, secondarySlotKeys);

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

        private void ClearSecondarySlots(EquipmentModel equipmentModel, List<EquipmentSlotKey> secondarySlotKeys)
        {
            foreach (var secondarySlot in secondarySlotKeys)
            {
                equipmentModel.RemoveFromSlot(secondarySlot, int.MaxValue);
            }
        }

        private bool CanStoreAllItems(IItemContainer<InventorySlot> container, ItemStackSnapshot oldItem, List<(EquipmentSlotKey slot, ItemStackSnapshot item)> secondarySlotItems)
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

        private bool RemoveOldEquipment(
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

        private void RollbackRemoveOldEquipment(
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

        private bool HandleUnequip(Entity entity, UnequipToContainerCommand c)
        {
            var equipmentModel = _itemContainerResolver.ResolveEquipment(entity);
            var toContainer = _itemContainerResolver.ResolveInventory(entity, c.ToContainer);
            var equipmentSlot = ContainerSlotConverter.ToEquipmentSlot(c.EquipmentSlot);

            var item = equipmentModel.Get(equipmentSlot);
            if (item == null) return false;

            // Move the item to container
            // Secondary slots are automatically unblocked once the item is removed
            return ItemContainerUtils.MoveAmount(equipmentModel, equipmentSlot, toContainer, item.Value.Amount) > 0;
        }

        private bool HandleDrop(Entity entity, DropItemCommand c)
        {
            if (!entity.TryGetModule<SpatialModule>(out var spatialModule)) return false;

            ItemStackSnapshot? item;
            int removedAmount;

            if (c.FromContainer == ItemContainerId.Equipment)
            {
                var equipment = _itemContainerResolver.ResolveEquipment(entity);
                var slot = ContainerSlotConverter.ToEquipmentSlot(c.FromSlot);
                item = equipment.Get(slot);
                if (item == null) return false;
                // Secondary slots are automatically unblocked once the item is removed
                removedAmount = ItemContainerUtils.RemoveAmount(equipment, slot, c.Amount);
            }
            else
            {
                var from = _itemContainerResolver.ResolveInventory(entity, c.FromContainer);
                var slot = ContainerSlotConverter.ToInventorySlot(c.FromSlot);
                item = from.Get(slot);
                if (item == null) return false;
                removedAmount = ItemContainerUtils.RemoveAmount(from, slot, c.Amount);
            }

            if (removedAmount > 0)
            {
                _globalEventBus.Publish(new LootItemDropRequestedEvent(spatialModule.Position, item.Value.Definition, removedAmount));
                return true;
            }
            return false;
        }

        private bool HandleUse(Entity entity, UseItemCommand c)
        {
            ItemStackSnapshot? item;

            if (c.Container == ItemContainerId.Equipment)
            {
                var equipment = _itemContainerResolver.ResolveEquipment(entity);
                item = equipment.Get(ContainerSlotConverter.ToEquipmentSlot(c.Slot));
            }
            else
            {
                var container = _itemContainerResolver.ResolveInventory(entity, c.Container);
                item = container.Get(ContainerSlotConverter.ToInventorySlot(c.Slot));
            }

            // Check if the item exists and has the Usable trait.
            if (item == null || !item.Value.Definition.TryGetTrait<UsableTrait>(out var usableTrait))
                return false;

            // Check cooldown and reset it if the item can be used.
            if (item.Value.InstanceData is not CooldownInstanceData cooldownTrait || !cooldownTrait.CooldownCounter.IsOver())
                return false;

            // Check for allowed and limiting RestrictionState
            if (entity.TryGetModule<RestrictionStateModule>(out var restrictionStateModule))
            {
                if (usableTrait.LimitingRestrictionState != RestrictionState.None &&
                    restrictionStateModule.Has(usableTrait.LimitingRestrictionState)) return false;
                if (usableTrait.RequiredRestrictionState != RestrictionState.None &&
                    !restrictionStateModule.Has(usableTrait.RequiredRestrictionState)) return false;
            }

            // Check if all functional traits allow triggering with the given context.
            var itemUseSettings = ResolveItemUseSettings(entity, c.IsManual);
            var triggerContext = new ItemTriggerContext(entity, ItemTrigger.OnUse, item.Value, itemUseSettings);
            if (item.Value.GetFunctionalTraits<FunctionalItemTraitBase>(ItemTrigger.OnUse).Any(t => !t.CanTrigger(triggerContext)))
                return false;

            // All checks passed, trigger the item use.
            cooldownTrait.CooldownCounter.Reset();
            entity.Publish(new ItemUseStartedEvent(item.Value, itemUseSettings));

            // If the item is consumable, remove one from the stack.
            if (usableTrait.Consumable)
            {
                if (c.Container == ItemContainerId.Equipment)
                {
                    var equipment = _itemContainerResolver.ResolveEquipment(entity);
                    equipment.RemoveFromSlot(ContainerSlotConverter.ToEquipmentSlot(c.Slot), 1);
                }
                else
                {
                    var container = _itemContainerResolver.ResolveInventory(entity, c.Container);
                    container.RemoveFromSlot(ContainerSlotConverter.ToInventorySlot(c.Slot), 1);
                }
            }

            return true;
        }
    }
}
