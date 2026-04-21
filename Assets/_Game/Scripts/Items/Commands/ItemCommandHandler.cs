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

                return true;
            }

            // --- 3) Swap path: the target equipment slot is occupied.
            var oldItemNullable = equipmentModel.Get(equipmentSlot);
            if (oldItemNullable == null)
                return false;

            var oldItem = oldItemNullable.Value;
            if (oldItem.Amount <= 0)
                return false;

            if (fromContainer.PreviewAdd(oldItem, AddPolicy.StackThenEmpty) < oldItem.Amount)
                return false;

            int removedOld = equipmentModel.RemoveFromSlot(equipmentSlot, oldItem.Amount);
            if (removedOld != oldItem.Amount)
                return false;

            if (fromContainer.RemoveFromSlot(fromSlot, fromItem.Amount) == 0)
            {
                equipmentModel.AddToSlot(equipmentSlot, oldItem);
                return false;
            }

            int equippedNew = equipmentModel.AddToSlot(equipmentSlot, fromItem);
            if (equippedNew == 0)
            {
                fromContainer.Add(fromItem, AddPolicy.StackThenEmpty);
                equipmentModel.AddToSlot(equipmentSlot, oldItem);
                return false;
            }

            int storedOld = fromContainer.Add(oldItem, AddPolicy.StackThenEmpty);
            if (storedOld != oldItem.Amount)
            {
                equipmentModel.RemoveFromSlot(equipmentSlot, fromItem.Amount);
                fromContainer.Add(fromItem, AddPolicy.StackThenEmpty);
                equipmentModel.AddToSlot(equipmentSlot, oldItem);
                throw new InvalidOperationException("Swap failed: cannot store old equipped item in source container.");
            }

            return true;
        }

        private bool HandleUnequip(Entity entity, UnequipToContainerCommand c)
        {
            var equipmentModel = _itemContainerResolver.ResolveEquipment(entity);
            var toContainer = _itemContainerResolver.ResolveInventory(entity, c.ToContainer);
            var equipmentSlot = ContainerSlotConverter.ToEquipmentSlot(c.EquipmentSlot);

            var item = equipmentModel.Get(equipmentSlot);
            if (item == null) return false;
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
            if (usableTrait.Cooldown <= 0 || item.Value.InstanceData is not CooldownInstanceData cooldownTrait || !cooldownTrait.CooldownCounter.IsOver())
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
