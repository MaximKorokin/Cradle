using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using System;

namespace Assets._Game.Scripts.Items
{
    public sealed class ItemContainerResolver
    {
        public InventoryModel ResolveInventory(Entity entity, ItemContainerId id)
        {
            return id switch
            {
                ItemContainerId.Inventory => entity.GetModule<InventoryModule>().Inventory,
                ItemContainerId.Storage => entity.GetModule<StorageModule>().Storage,
                _ => throw new NotSupportedException($"Container {id} is not an inventory."),
            };
        }

        public EquipmentModel ResolveEquipment(Entity entity)
        {
            return entity.GetModule<EquipmentModule>().Equipment;
        }

        public IItemContainer ResolveContainer(Entity entity, ItemContainerId id)
        {
            return id switch
            {
                ItemContainerId.Inventory => ResolveInventory(entity, id),
                ItemContainerId.Equipment => ResolveEquipment(entity),
                ItemContainerId.Storage => ResolveInventory(entity, id),
                _ => throw new NotSupportedException($"Container {id} is not supported."),
            };
        }
    }
}
