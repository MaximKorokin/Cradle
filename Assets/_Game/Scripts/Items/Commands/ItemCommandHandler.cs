using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Configs;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.Items.Shop;
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
        private readonly EntityRepository _entityRepository;
        private readonly ItemContainerResolver _itemContainerResolver;
        private readonly ItemsConfig _itemsConfig;

        public ItemCommandHandler(
            IGlobalEventBus globalEventBus,
            EntityRepository entityRepository,
            ItemContainerResolver itemContainerResolver,
            ItemsConfig itemsConfig)
        {
            _globalEventBus = globalEventBus;
            _entityRepository = entityRepository;
            _itemContainerResolver = itemContainerResolver;
            _itemsConfig = itemsConfig;
        }

        public bool Handle(IItemCommand command)
        {
            return command switch
            {
                TransferToContainerCommand c => HandleTransfer(c),
                TransferToContainerSlotCommand c => HandleTransferToSlot(c),
                EquipFromContainerCommand c => HandleEquip(c),
                UnequipToContainerCommand c => HandleUnequip(c),
                DestroyItemCommand c => HandleDestroy(c),
                DropItemCommand c => HandleDrop(c),
                UseItemCommand c => HandleUse(c),
                BuyFromShopCommand c => HandleBuy(c),
                SellToShopCommand c => HandleSell(c),
                EnchantItemCommand c => HandleEnchant(c),
                _ => throw new NotSupportedException(command.GetType().Name),
            };
        }

        private bool HandleTransfer(TransferToContainerCommand c)
        {
            var from = _itemContainerResolver.ResolveInventory(c.FromContainer);
            var to = _itemContainerResolver.ResolveContainer(c.ToContainer);
            return ItemContainerUtils.MoveAmount(from, ContainerSlotConverter.ToInventorySlot(c.FromSlot), to, c.Amount) > 0;
        }

        private bool HandleTransferToSlot(TransferToContainerSlotCommand c)
        {
            var from = _itemContainerResolver.ResolveContainer(c.FromContainer);
            var to = _itemContainerResolver.ResolveContainer(c.ToContainer);

            var item = from.Get(c.FromSlot);
            if (item != null && item.Value.Amount == c.Amount)
            {
                // If the entire stack is being moved, try to swap the items instead of moving them
                return ItemContainerUtils.TrySwapBetweenContainerSlots(from, c.FromSlot, to, c.ToSlot);
            }

            return ItemContainerUtils.MoveAmount(
                from,
                c.FromSlot,
                to,
                c.ToSlot,
                c.Amount) > 0;
        }

        private bool HandleEquip(EquipFromContainerCommand c)
        {
            var fromContainer = _itemContainerResolver.ResolveInventory(c.FromContainer);
            var fromSlot = ContainerSlotConverter.ToInventorySlot(c.FromSlot);
            var equipmentModel = _itemContainerResolver.ResolveEquipment(c.Equipment);
            var equipmentSlot = ContainerSlotConverter.ToEquipmentSlot(c.EquipmentSlot);

            var fromItemNullable = fromContainer.Get(fromSlot);
            if (fromItemNullable == null)
                return false;

            var fromItem = fromItemNullable.Value;
            if (fromItem.Amount <= 0)
                return false;

            return ItemContainerUtils.TryEquipWithSwap(fromContainer, fromSlot, equipmentSlot, equipmentModel, fromItem);
        }

        private bool HandleUnequip(UnequipToContainerCommand c)
        {
            var equipmentModel = _itemContainerResolver.ResolveEquipment(c.FromEquipment);
            var toContainer = _itemContainerResolver.ResolveInventory(c.ToContainer);
            var equipmentSlot = ContainerSlotConverter.ToEquipmentSlot(c.EquipmentSlot);

            var item = equipmentModel.Get(equipmentSlot);
            if (item == null) return false;

            // Move the item to container
            // Secondary slots are automatically unblocked once the item is removed
            return ItemContainerUtils.MoveAmount(equipmentModel, equipmentSlot, toContainer, item.Value.Amount) > 0;
        }

        private bool HandleDestroy(DestroyItemCommand c)
        {
            if (c.FromContainer.ContainerId == ItemContainerId.Equipment)
            {
                var equipmentModel = _itemContainerResolver.ResolveEquipment(c.FromContainer);
                var slot = ContainerSlotConverter.ToEquipmentSlot(c.FromSlot);
                return ItemContainerUtils.RemoveAmount(equipmentModel, slot, c.Amount) > 0;
            }
            else
            {
                var container = _itemContainerResolver.ResolveInventory(c.FromContainer);
                var slot = ContainerSlotConverter.ToInventorySlot(c.FromSlot);
                return ItemContainerUtils.RemoveAmount(container, slot, c.Amount) > 0;
            }
        }

        private bool HandleDrop(DropItemCommand c)
        {
            var entity = _entityRepository.Get(c.FromContainer.EntityId);
            if (!entity.TryGetModule<SpatialModule>(out var spatialModule)) return false;

            ItemStackSnapshot? item;
            int removedAmount;

            if (c.FromContainer.ContainerId == ItemContainerId.Equipment)
            {
                var equipmentModel = _itemContainerResolver.ResolveEquipment(c.FromContainer);
                var slot = ContainerSlotConverter.ToEquipmentSlot(c.FromSlot);
                item = equipmentModel.Get(slot);
                if (item == null) return false;
                // Secondary slots are automatically unblocked once the item is removed
                removedAmount = ItemContainerUtils.RemoveAmount(equipmentModel, slot, c.Amount);
            }
            else
            {
                var from = _itemContainerResolver.ResolveInventory(c.FromContainer);
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

        private bool HandleUse(UseItemCommand c)
        {
            ItemStackSnapshot? item;

            if (c.Container.ContainerId == ItemContainerId.Equipment)
            {
                var equipment = _itemContainerResolver.ResolveEquipment(c.Container);
                item = equipment.Get(ContainerSlotConverter.ToEquipmentSlot(c.Slot));
            }
            else
            {
                var container = _itemContainerResolver.ResolveInventory(c.Container);
                item = container.Get(ContainerSlotConverter.ToInventorySlot(c.Slot));
            }

            // Check if the item exists and has the Usable trait.
            if (item == null || !item.Value.Definition.TryGetTrait<UsableTrait>(out var usableTrait))
                return false;

            // Check cooldown and reset it if the item can be used.
            if (!item.Value.InstanceData.TryGet<CooldownInstanceData>(out var cooldownData) || !cooldownData.CooldownCounter.IsOver())
                return false;

            // Check for allowed and limiting RestrictionState
            var entity = _entityRepository.Get(c.Container.EntityId);
            if (entity.TryGetModule<RestrictionStateModule>(out var restrictionStateModule))
            {
                if (usableTrait.LimitingRestrictionState != RestrictionState.None &&
                    restrictionStateModule.Has(usableTrait.LimitingRestrictionState)) return false;
                if (usableTrait.RequiredRestrictionState != RestrictionState.None &&
                    !restrictionStateModule.Has(usableTrait.RequiredRestrictionState)) return false;
            }

            // Check if all functional traits allow triggering with the given context.

            var equipmentModule = entity.GetModule<EquipmentModule>();
            var itemUseSettings = c.IsManual ? equipmentModule.ManualItemUseSettings : equipmentModule.AutoItemUseSettings;
            var triggerContext = new ItemTriggerContext(entity, ItemTrigger.OnUse, item.Value, itemUseSettings);
            if (item.Value.GetFunctionalTraits<FunctionalItemTraitBase>(ItemTrigger.OnUse).Any(t => !t.CanTrigger(triggerContext)))
                return false;

            // All checks passed, trigger the item use.
            cooldownData.CooldownCounter.Reset();
            entity.Publish(new ItemUseStartedEvent(item.Value, itemUseSettings));

            // If the item is consumable, remove one from the stack.
            if (usableTrait.Consumable)
            {
                if (c.Container.ContainerId == ItemContainerId.Equipment)
                {
                    var equipment = _itemContainerResolver.ResolveEquipment(c.Container);
                    equipment.RemoveFromSlot(ContainerSlotConverter.ToEquipmentSlot(c.Slot), 1);
                }
                else
                {
                    var container = _itemContainerResolver.ResolveInventory(c.Container);
                    container.RemoveFromSlot(ContainerSlotConverter.ToInventorySlot(c.Slot), 1);
                }
            }

            return true;
        }

        private bool HandleBuy(BuyFromShopCommand c)
        {
            var inventoryModel = _itemContainerResolver.ResolveInventory(c.InventoryModelPath);
            var shopModel = _itemContainerResolver.ResolveShop(c.ShopModelPath);

            var itemSnapshot = shopModel.Get(ShopSlot.FromInt64(c.ShopSlot));
            if (!itemSnapshot.HasValue) return false;

            var goldDefinition = _itemsConfig.GetSpecialItemDefinition(SpecialItemId.Gold);
            var goldKey = ItemKey.From(goldDefinition, null);
            if (inventoryModel.Count(goldKey) < c.Price)
                return false;

            // Try to add to inventory
            var amountToTransfer = Math.Min(c.Amount, itemSnapshot.Value.Amount);
            var preview = inventoryModel.PreviewAdd(new ItemStackSnapshot(
                itemSnapshot.Value.Definition,
                itemSnapshot.Value.InstanceData,
                amountToTransfer));

            if (preview != amountToTransfer) return false;

            // Remove gold from inventory
            inventoryModel.Remove(goldKey, c.Price);

            // Remove item from shop
            shopModel.RemoveFromSlot(ShopSlot.FromInt64(c.ShopSlot), preview);

            // Add item to inventory
            inventoryModel.Add(new ItemStackSnapshot(
                itemSnapshot.Value.Definition,
                itemSnapshot.Value.InstanceData,
                preview));

            return true;
        }

        private bool HandleSell(SellToShopCommand c)
        {
            var inventoryModel = _itemContainerResolver.ResolveInventory(c.InventoryModelPath);
            var shopModel = _itemContainerResolver.ResolveShop(c.ShopModelPath);

            var itemSnapshot = inventoryModel.Get(InventorySlot.FromInt64(c.InventorySlot));
            if (!itemSnapshot.HasValue) return false;

            // Remove item from inventory
            var removed = inventoryModel.RemoveFromSlot(InventorySlot.FromInt64(c.InventorySlot), c.Amount);
            if (removed <= 0) return false;

            var goldDefinition = _itemsConfig.GetSpecialItemDefinition(SpecialItemId.Gold);
            inventoryModel.Add(new ItemStackSnapshot(goldDefinition, null, c.Price));

            // Add item back to shop (buyback feature)
            shopModel.Add(new ItemStackSnapshot(
                itemSnapshot.Value.Definition,
                itemSnapshot.Value.InstanceData,
                removed));

            return true;
        }

        private bool HandleEnchant(EnchantItemCommand c)
        {
            var containerModel = _itemContainerResolver.ResolveContainer(c.FromContainer);
            var itemSnapshot = containerModel.Get(c.FromSlot);

            if (!itemSnapshot.HasValue) return false;
            if (!itemSnapshot.Value.Definition.TryGetTrait<EnchantableTrait>(out var enchantableTrait)) return false;
            if (!itemSnapshot.Value.InstanceData.TryGet<EnchantInstanceData>(out var enchantInstanceData)) return false;
            if (enchantInstanceData.Level >= enchantableTrait.ItemEnchantingDefinition.Levels.Count) return false;

            var inventoryPath = ItemContainerPath.Inventory(c.FromContainer.EntityId);
            var enchantingItemKey = ItemKey.From(enchantableTrait.ItemEnchantingDefinition.Methods[0].Item, null);
            var inventoryModel = _itemContainerResolver.ResolveInventory(inventoryPath);
            if (!inventoryModel.Has(enchantingItemKey, 1)) return false;

            // Checks passed, remove enchanting item from inventory and perform enchantment
            inventoryModel.Remove(enchantingItemKey, 1);

            var isEnchantSuccessful = enchantableTrait.ItemEnchantingDefinition.Methods[0].Rules[enchantInstanceData.Level].SuccessChance > UnityEngine.Random.value;
            if (isEnchantSuccessful)
            {
                enchantInstanceData.Level++;
                return true;
            }

            // Enchant failed, apply failure rules
            switch (enchantableTrait.ItemEnchantingDefinition.Methods[0].Rules[enchantInstanceData.Level].Failure.Type)
            {
                case Enchanting.EnchantFailureType.None:
                    break;
                case Enchanting.EnchantFailureType.Downgrade:
                    if (enchantInstanceData.Level > 0) enchantInstanceData.Level--;
                    break;
                case Enchanting.EnchantFailureType.Reset:
                    enchantInstanceData.Level = 0;
                    break;
                case Enchanting.EnchantFailureType.Destroy:
                    var itemKey = ItemKey.From(itemSnapshot.Value.Definition, itemSnapshot.Value.InstanceData);
                    containerModel.Remove(itemKey, 1);
                    break;
            }
            return true;
        }
    }
}
