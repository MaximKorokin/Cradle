using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.Items.Shop;
using System;

namespace Assets._Game.Scripts.Items
{
    public sealed class ItemContainerResolver
    {
        private readonly EntityRepository _entityManager;

        public ItemContainerResolver(EntityRepository entityRepository)
        {
            _entityManager = entityRepository;
        }

        public IItemContainer ResolveContainer(ItemContainerPath path)
        {
            var entity = _entityManager.Get(path.EntityId);
            return ResolveContainer(entity, path.ContainerId);
        }

        public InventoryModel ResolveInventory(ItemContainerPath path)
        {
            var entity = _entityManager.Get(path.EntityId);
            return ResolveInventory(entity, path.ContainerId);
        }

        public EquipmentModel ResolveEquipment(ItemContainerPath path)
        {
            return ResolveEquipment(path.EntityId);
        }

        public ShopModel ResolveShop(ItemContainerPath path)
        {
            return ResolveShop(path.EntityId);
        }

        private IItemContainer ResolveContainer(Entity entity, ItemContainerId id)
        {
            return id switch
            {
                ItemContainerId.Inventory => ResolveInventory(entity, id),
                ItemContainerId.Equipment => ResolveEquipment(entity),
                ItemContainerId.Storage => ResolveInventory(entity, id),
                ItemContainerId.Shop => ResolveShop(entity),
                _ => throw new ArgumentException($"Container {id} is not supported."),
            };
        }

        private InventoryModel ResolveInventory(Entity entity, ItemContainerId id)
        {
            return id switch
            {
                ItemContainerId.Inventory => entity.GetModule<InventoryModule>().Inventory,
                ItemContainerId.Storage => entity.GetModule<StorageModule>().Storage,
                _ => throw new NotSupportedException($"Container {id} is not an inventory."),
            };
        }

        public EquipmentModel ResolveEquipment(string entityId)
        {
            var entity = _entityManager.Get(entityId);
            return ResolveEquipment(entity);
        }

        private EquipmentModel ResolveEquipment(Entity entity)
        {
            if (!entity.TryGetModule<EquipmentModule>(out var equipmentModule))
                throw new InvalidOperationException($"Entity {entity.Id} does not have an equipment module.");
            return equipmentModule.Equipment;
        }

        public ShopModel ResolveShop(string entityId)
        {
            var entity = _entityManager.Get(entityId);
            return ResolveShop(entity);
        }

        private ShopModel ResolveShop(Entity entity)
        {
            if (!entity.TryGetModule<ShopModule>(out var shopModule))
                throw new InvalidOperationException($"Entity {entity.Id} does not have a shop module.");
            return shopModule.Shop;
        }
    }
}
