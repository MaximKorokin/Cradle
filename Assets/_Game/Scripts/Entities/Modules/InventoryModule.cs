using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Inventory;

namespace Assets._Game.Scripts.Entities.Modules
{
    public class InventoryModule : EntityModuleBase
    {
        public InventoryModel Inventory { get; private set; }

        public InventoryModule(InventoryModel inventory)
        {
            Inventory = inventory;

            if (Inventory != null) Inventory.InventoryChanged += OnInventorySlotChanged;
        }

        public override void Dispose()
        {
            base.Dispose();

            if (Inventory != null) Inventory.InventoryChanged -= OnInventorySlotChanged;
        }

        private void OnInventorySlotChanged(InventoryChange inventoryChange)
        {
            Publish(new InventoryChangedEvent(inventoryChange));
        }
    }

    public readonly struct InventoryChangedEvent : IEntityEvent
    {
        public readonly InventorySlot Slot;
        public readonly ItemStackSnapshot? Item;
        public readonly InventoryChangeKind Kind;

        public InventoryChangedEvent(InventorySlot slot, ItemStackSnapshot? item, InventoryChangeKind kind)
        {
            Slot = slot;
            Item = item;
            Kind = kind;
        }

        public InventoryChangedEvent(InventoryChange inventoryChange)
        {
            Slot = inventoryChange.Slot;
            Item = inventoryChange.Item;
            Kind = inventoryChange.Kind;
        }
    }

    public class InventoryModuleFactory : IEntityModuleFactory, IEntityModulePersistance
    {
        private readonly InventoryModelFactory _inventoryModelAssembler;

        public InventoryModuleFactory(InventoryModelFactory inventoryModelAssembler)
        {
            _inventoryModelAssembler = inventoryModelAssembler;
        }

        public EntityModuleBase Create(EntityDefinition entityDefinition)
        {
            if (!entityDefinition.TryGetModuleDefinition<InventoryModuleDefinition>(out var inventoryDefinitionModule))
                return null;

            var inventoryModel = _inventoryModelAssembler.Create(inventoryDefinitionModule.SlotsAmount);
            return new InventoryModule(inventoryModel);
        }

        public void Apply(Entity entity, EntitySave entitySave)
        {
            if (!entity.TryGetModule<InventoryModule>(out var inventoryModule)) return;
            _inventoryModelAssembler.Apply(inventoryModule.Inventory, entitySave.InventorySave);
        }

        public void Save(Entity entity, EntitySave entitySave)
        {
            if (!entity.TryGetModule<InventoryModule>(out var inventoryModule)) return;
            entitySave.InventorySave = _inventoryModelAssembler.Save(inventoryModule.Inventory);
        }
    }
}
