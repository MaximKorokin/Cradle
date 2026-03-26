using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets._Game.Scripts.Items.Inventory;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class StorageModule : EntityModuleBase
    {
        public InventoryModel Storage { get; private set; }

        public StorageModule(InventoryModel storage)
        {
            Storage = storage;

            if (Storage != null) Storage.InventoryChanged += OnInventorySlotChanged;
        }

        public override void Dispose()
        {
            base.Dispose();

            if (Storage != null) Storage.InventoryChanged -= OnInventorySlotChanged;
        }

        private void OnInventorySlotChanged(InventoryChange equipmentChange)
        {

        }
    }

    public class StorageModuleFactory : IEntityModuleFactory, IEntityModulePersistance
    {
        private readonly InventoryModelAssembler _inventoryModelAssembler;

        public StorageModuleFactory(InventoryModelAssembler inventoryModelAssembler)
        {
            _inventoryModelAssembler = inventoryModelAssembler;
        }

        public EntityModuleBase Create(EntityDefinition entityDefinition)
        {
            InventoryModel inventoryModel = null;
            if (entityDefinition.TryGetModuleDefinition<StorageModuleDefinition>(out var storageModuleDefinition))
            {
                inventoryModel = _inventoryModelAssembler.Create(storageModuleDefinition.SlotsAmount);
            }

            return new StorageModule(inventoryModel);
        }

        public void Apply(Entity entity, EntitySave entitySave)
        {
            if (!entity.TryGetModule<StorageModule>(out var storageModule)) return;
            _inventoryModelAssembler.Apply(storageModule.Storage, entitySave.StorageSave);
        }

        public void Save(Entity entity, EntitySave entitySave)
        {
            if (!entity.TryGetModule<StorageModule>(out var storageModule)) return;
            entitySave.StorageSave = _inventoryModelAssembler.Save(storageModule.Storage);
        }
    }
}
