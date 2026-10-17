using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Configs;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Infrastructure.Systems.GameAnalytics;
using Assets._Game.Scripts.Items.Equipment;
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
                AddItemCommand c => HandleAdd(c),
                DestroyItemCommand c => HandleDestroy(c),
                DropItemCommand c => HandleDrop(c),
                UseItemCommand c => HandleUse(c),
                BuyFromShopCommand c => HandleBuy(c),
                SellToShopCommand c => HandleSell(c),
                EnchantItemCommand c => HandleEnchant(c),
                _ => throw new NotSupportedException(command.GetType().Name),
            };
        }

        private void PublishGameAnalyticsEvent(AnalyticsEntryType type, ItemContainerPath fromPath, ItemContainerPath toPath, ItemDefinition itemDefinition, int amount, string details = null)
        {
            Entity fromEntity = null;
            if (fromPath.HasData && !string.IsNullOrWhiteSpace(fromPath.EntityId)) _entityRepository.TryGet(fromPath.EntityId, out fromEntity);
            Entity toEntity = null;
            if (toPath.HasData && !string.IsNullOrWhiteSpace(toPath.EntityId)) _entityRepository.TryGet(toPath.EntityId, out toEntity);

            var name = itemDefinition != null ? itemDefinition.Name : null;
            _globalEventBus.Publish(new GameAnalyticsEvent(type, fromEntity, toEntity, amount, details == null ? name : $"{name} {details}"));
        }

        private IDisposable BeginStatTransaction(ItemContainerPath path)
        {
            if (!path.HasData || string.IsNullOrWhiteSpace(path.EntityId) ||
                !_entityRepository.TryGet(path.EntityId, out var entity) ||
                !entity.TryGetModule<StatModule>(out var statModule))
                return null;

            return statModule.Stats.BeginTransaction();
        }

        private bool HandleTransfer(TransferToContainerCommand c)
        {
            using (BeginStatTransaction(c.FromContainer))
            using (BeginStatTransaction(c.ToContainer))
            {
                var from = _itemContainerResolver.ResolveContainer<InventoryModel>(c.FromContainer);
                var to = _itemContainerResolver.ResolveContainer(c.ToContainer);
                var item = from.Get(ContainerSlotConverter.ToInventorySlot(c.FromSlot));

                int moved;
                if (to is EquipmentModel equipmentModel)
                {
                    var fromSlot = ContainerSlotConverter.ToInventorySlot(c.FromSlot);
                    moved = ItemContainerUtils.MoveToEquipmentSlot(from, fromSlot, equipmentModel, null, c.Amount);
                    if (moved > 0) PublishGameAnalyticsEvent(AnalyticsEntryType.ItemEquip, c.FromContainer, c.ToContainer, item?.Definition, moved);
                    return moved > 0;
                }

                moved = ItemContainerUtils.MoveAmount(from, ContainerSlotConverter.ToInventorySlot(c.FromSlot), to, c.Amount);
                if (moved > 0) PublishGameAnalyticsEvent(AnalyticsEntryType.ItemTransfer, c.FromContainer, c.ToContainer, item?.Definition, moved);
                return moved > 0;
            }
        }

        private bool HandleTransferToSlot(TransferToContainerSlotCommand c)
        {
            using (BeginStatTransaction(c.FromContainer))
            using (BeginStatTransaction(c.ToContainer))
            {
                var from = _itemContainerResolver.ResolveContainer(c.FromContainer);
                var to = _itemContainerResolver.ResolveContainer(c.ToContainer);

                var item = from.Get(c.FromSlot);
                if (from is InventoryModel inventoryModel && to is EquipmentModel equipmentModel)
                {
                    var fromSlot = ContainerSlotConverter.ToInventorySlot(c.FromSlot);
                    var equipmentSlot = ContainerSlotConverter.ToEquipmentSlot(c.ToSlot);
                    var equipped = ItemContainerUtils.MoveToEquipmentSlot(inventoryModel, fromSlot, equipmentModel, equipmentSlot, c.Amount);
                    if (equipped > 0) PublishGameAnalyticsEvent(AnalyticsEntryType.ItemEquip, c.FromContainer, c.ToContainer, item?.Definition, equipped);
                    return equipped > 0;
                }

                bool success;
                int movedAmount = c.Amount;
                if (item != null && item.Value.Amount == c.Amount)
                {
                    // If the entire stack is being moved, try to swap the items instead of moving them
                    success = ItemContainerUtils.TrySwapBetweenContainerSlots(from, c.FromSlot, to, c.ToSlot);
                }
                else
                {
                    movedAmount = ItemContainerUtils.MoveAmount(from, c.FromSlot, to, c.ToSlot, c.Amount);
                    success = movedAmount > 0;
                }

                if (success) PublishGameAnalyticsEvent(AnalyticsEntryType.ItemTransfer, c.FromContainer, c.ToContainer, item?.Definition, movedAmount);
                return success;
            }
        }

        private bool HandleEquip(EquipFromContainerCommand c)
        {
            using (BeginStatTransaction(c.FromContainer))
            using (BeginStatTransaction(c.Equipment))
            {
                var fromContainer = _itemContainerResolver.ResolveContainer<InventoryModel>(c.FromContainer);
                var fromSlot = ContainerSlotConverter.ToInventorySlot(c.FromSlot);
                var equipmentModel = _itemContainerResolver.ResolveContainer<EquipmentModel>(c.Equipment);
                var equipmentSlot = ContainerSlotConverter.ToEquipmentSlot(c.EquipmentSlot);

                var item = fromContainer.Get(fromSlot);
                var equipped = ItemContainerUtils.MoveToEquipmentSlot(fromContainer, fromSlot, equipmentModel, equipmentSlot, null);
                if (equipped > 0) PublishGameAnalyticsEvent(AnalyticsEntryType.ItemEquip, c.FromContainer, c.Equipment, item?.Definition, equipped);
                return equipped > 0;
            }
        }

        private bool HandleUnequip(UnequipToContainerCommand c)
        {
            using (BeginStatTransaction(c.FromEquipment))
            using (BeginStatTransaction(c.ToContainer))
            {
                var equipmentModel = _itemContainerResolver.ResolveContainer<EquipmentModel>(c.FromEquipment);
                var toContainer = _itemContainerResolver.ResolveContainer<InventoryModel>(c.ToContainer);
                var equipmentSlot = ContainerSlotConverter.ToEquipmentSlot(c.EquipmentSlot);

                var item = equipmentModel.Get(equipmentSlot);
                if (item == null) return false;

                // Move the item to container
                // Secondary slots are automatically unblocked once the item is removed
                var unequipped = ItemContainerUtils.MoveAmount(equipmentModel, equipmentSlot, toContainer, item.Value.Amount);
                if (unequipped > 0) PublishGameAnalyticsEvent(AnalyticsEntryType.ItemUnequip, c.FromEquipment, c.ToContainer, item?.Definition, unequipped);
                return unequipped > 0;
            }
        }

        private bool HandleAdd(AddItemCommand c)
        {
            using (BeginStatTransaction(c.ToContainer))
            {
                var container = _itemContainerResolver.ResolveContainer<InventoryModel>(c.ToContainer);
                container.Add(c.Item);
                PublishGameAnalyticsEvent(AnalyticsEntryType.ItemAppear, default, c.ToContainer, c.Item.Definition, c.Item.Amount);
                return true;
            }
        }

        private bool HandleDestroy(DestroyItemCommand c)
        {
            var container = _itemContainerResolver.ResolveContainer(c.FromContainer);
            var item = container.Get(c.FromSlot);
            var removed = ItemContainerUtils.RemoveAmount(container, c.FromSlot, c.Amount);
            if (removed > 0) PublishGameAnalyticsEvent(AnalyticsEntryType.ItemDestroy, c.FromContainer, default, item?.Definition, removed);
            return removed > 0;
        }

        private bool HandleDrop(DropItemCommand c)
        {
            var entity = _entityRepository.Get(c.FromContainer.EntityId);
            if (!entity.TryGetModule<SpatialModule>(out var spatialModule)) return false;

            var from = _itemContainerResolver.ResolveContainer(c.FromContainer);
            var item = from.Get(c.FromSlot);
            if (item == null) return false;
            var removedAmount = ItemContainerUtils.RemoveAmount(from, c.FromSlot, c.Amount);

            if (removedAmount > 0)
            {
                _globalEventBus.Publish(new LootItemDropRequestedEvent(spatialModule.Position, item.Value.Definition, removedAmount));
                PublishGameAnalyticsEvent(AnalyticsEntryType.ItemDrop, c.FromContainer, default, item?.Definition, removedAmount);
                return true;
            }
            return false;
        }

        private bool HandleUse(UseItemCommand c)
        {
            ItemStackSnapshot? item;

            if (c.Container.ContainerId == ItemContainerId.Equipment)
            {
                var equipment = _itemContainerResolver.ResolveContainer<EquipmentModel>(c.Container);
                item = equipment.Get(ContainerSlotConverter.ToEquipmentSlot(c.Slot));
            }
            else
            {
                var container = _itemContainerResolver.ResolveContainer<InventoryModel>(c.Container);
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
            PublishGameAnalyticsEvent(AnalyticsEntryType.ItemUse, c.Container, default, item?.Definition, 1);

            // If the item is consumable, remove one from the stack.
            if (usableTrait.Consumable)
            {
                if (c.Container.ContainerId == ItemContainerId.Equipment)
                {
                    var equipment = _itemContainerResolver.ResolveContainer<EquipmentModel>(c.Container);
                    equipment.RemoveFromSlot(ContainerSlotConverter.ToEquipmentSlot(c.Slot), 1);
                }
                else
                {
                    var container = _itemContainerResolver.ResolveContainer<InventoryModel>(c.Container);
                    container.RemoveFromSlot(ContainerSlotConverter.ToInventorySlot(c.Slot), 1);
                }
            }

            return true;
        }

        private bool HandleBuy(BuyFromShopCommand c)
        {
            var itemContainerModel = _itemContainerResolver.ResolveContainer(c.ItemContainerModelPath);
            var goldInventoryModel = _itemContainerResolver.ResolveContainer<InventoryModel>(c.GoldInventoryModelPath);
            var shopModel = _itemContainerResolver.ResolveContainer<ShopModel>(c.ShopModelPath);

            var itemSnapshot = shopModel.Get(ShopSlot.FromInt64(c.ShopSlot));
            if (!itemSnapshot.HasValue) return false;

            var goldDefinition = _itemsConfig.GetSpecialItemDefinition(SpecialItemId.Gold);
            var goldKey = ItemKey.From(goldDefinition, null);
            if (goldInventoryModel.Count(goldKey) < c.Price)
                return false;

            // Try to add to inventory
            var amountToTransfer = Math.Min(c.Amount, itemSnapshot.Value.Amount);
            var preview = itemContainerModel.PreviewAddToSlot(
                c.InventorySlot,
                new ItemStackSnapshot(
                    itemSnapshot.Value.Definition,
                    itemSnapshot.Value.InstanceData,
                    amountToTransfer));

            if (preview != amountToTransfer) return false;

            // Remove gold from inventory
            goldInventoryModel.Remove(goldKey, c.Price);

            PublishGameAnalyticsEvent(AnalyticsEntryType.ItemDisappear, c.GoldInventoryModelPath, c.ShopModelPath, goldDefinition, c.Price);

            // Remove item from shop
            shopModel.RemoveFromSlot(ShopSlot.FromInt64(c.ShopSlot), preview);

            // Add item to inventory
            itemContainerModel.AddToSlot(
                c.InventorySlot,
                new ItemStackSnapshot(
                    itemSnapshot.Value.Definition,
                    itemSnapshot.Value.InstanceData,
                    preview));

            PublishGameAnalyticsEvent(AnalyticsEntryType.ItemBuy, c.ShopModelPath, c.ItemContainerModelPath, itemSnapshot?.Definition, preview);
            return true;
        }

        private bool HandleSell(SellToShopCommand c)
        {
            var itemContainerModel = _itemContainerResolver.ResolveContainer(c.ItemContainerModelPath);
            var goldInventoryModel = _itemContainerResolver.ResolveContainer<InventoryModel>(c.GoldInventoryModelPath);
            var shopModel = _itemContainerResolver.ResolveContainer<ShopModel>(c.ShopModelPath);

            var itemSnapshot = itemContainerModel.Get(c.InventorySlot);
            if (!itemSnapshot.HasValue) return false;

            // Remove item from inventory
            var removed = itemContainerModel.RemoveFromSlot(c.InventorySlot, c.Amount);
            if (removed <= 0) return false;

            var goldDefinition = _itemsConfig.GetSpecialItemDefinition(SpecialItemId.Gold);
            var goldSnapshot = new ItemStackSnapshot(goldDefinition, null, c.Price);
            goldInventoryModel.Add(goldSnapshot);
            PublishGameAnalyticsEvent(AnalyticsEntryType.ItemAppear, c.ShopModelPath, c.GoldInventoryModelPath, goldDefinition, c.Price);

            // Add item back to shop (buyback feature)
            shopModel.Add(new ItemStackSnapshot(
                itemSnapshot.Value.Definition,
                itemSnapshot.Value.InstanceData,
                removed));

            PublishGameAnalyticsEvent(AnalyticsEntryType.ItemSell, c.ItemContainerModelPath, c.ShopModelPath, itemSnapshot?.Definition, removed);
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
            var enchantingItemDefinition = enchantableTrait.ItemEnchantingDefinition.Methods[0].Item;
            var enchantingItemKey = ItemKey.From(enchantingItemDefinition, null);
            var inventoryModel = _itemContainerResolver.ResolveContainer<InventoryModel>(inventoryPath);
            if (!inventoryModel.Has(enchantingItemKey, 1)) return false;

            // Checks passed, remove enchanting item from inventory and perform enchantment
            inventoryModel.Remove(enchantingItemKey, 1);
            PublishGameAnalyticsEvent(AnalyticsEntryType.ItemDisappear, c.FromContainer, default, enchantingItemDefinition, 1);

            var isEnchantSuccessful = enchantableTrait.ItemEnchantingDefinition.Methods[0].Rules[enchantInstanceData.Level].SuccessChance > UnityEngine.Random.value;
            if (isEnchantSuccessful)
            {
                enchantInstanceData.Level++;
                PublishGameAnalyticsEvent(AnalyticsEntryType.ItemEnchantSucces, c.FromContainer, default, itemSnapshot?.Definition, 1, $"level {enchantInstanceData.Level}");
                return true;
            }

            // Enchant failed, apply failure rules
            PublishGameAnalyticsEvent(AnalyticsEntryType.ItemEnchantFail, c.FromContainer, default, itemSnapshot?.Definition, 1, enchantableTrait.ItemEnchantingDefinition.Methods[0].Rules[enchantInstanceData.Level].Failure.Type.ToString());
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
                    PublishGameAnalyticsEvent(AnalyticsEntryType.ItemDisappear, c.FromContainer, default, itemSnapshot?.Definition, 1);
                    break;
            }
            return true;
        }
    }
}
