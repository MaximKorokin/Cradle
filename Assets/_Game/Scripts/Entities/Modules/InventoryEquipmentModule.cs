using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using System.Linq;

namespace Assets._Game.Scripts.Entities.Modules
{
    public class InventoryEquipmentModule : EntityModuleBase
    {
        public InventoryEquipmentModule(InventoryModel inventory, EquipmentModel equipment)
        {
            Inventory = inventory;
            Equipment = equipment;

            if (Equipment != null) Equipment.EquipmentChanged += OnEquipmentSlotChanged;
            if (Inventory != null) Inventory.InventoryChanged += OnInventorySlotChanged;
        }

        public InventoryModel Inventory { get; private set; }
        public EquipmentModel Equipment { get; private set; }

        private void OnEquipmentSlotChanged(EquipmentChange equipmentChange)
        {
            Publish(new EquipmentChangedEvent(Entity, equipmentChange));
        }

        private void OnInventorySlotChanged(InventoryChange equipmentChange)
        {
            Publish(new InventoryChangedEvent(Entity, equipmentChange));
        }

        public override void Dispose()
        {
            base.Dispose();
            if (Equipment != null) Equipment.EquipmentChanged -= OnEquipmentSlotChanged;
            if (Inventory != null) Inventory.InventoryChanged -= OnInventorySlotChanged;
        }
    }

    public readonly struct EquipmentChangedEvent : IEntityEvent
    {
        public readonly EquipmentSlotKey Slot;
        public readonly ItemStackSnapshot? Item;
        public readonly EquipmentChangeKind Kind;

        public Entity Entity { get; }

        public EquipmentChangedEvent(Entity entity, EquipmentSlotKey slot, ItemStackSnapshot? item, EquipmentChangeKind kind)
        {
            Entity = entity;
            Slot = slot;
            Item = item;
            Kind = kind;
        }

        public EquipmentChangedEvent(Entity entity, EquipmentChange equipmentChange)
        {
            Entity = entity;
            Slot = equipmentChange.Slot;
            Item = equipmentChange.Item;
            Kind = equipmentChange.Kind;
        }
    }

    public readonly struct InventoryChangedEvent : IEntityEvent
    {
        public readonly int Slot;
        public readonly ItemStackSnapshot? Item;
        public readonly InventoryChangeKind Kind;

        public Entity Entity { get; }

        public InventoryChangedEvent(Entity entity, int slot, ItemStackSnapshot? item, InventoryChangeKind kind)
        {
            Entity = entity;
            Slot = slot;
            Item = item;
            Kind = kind;
        }

        public InventoryChangedEvent(Entity entity, InventoryChange inventoryChange)
        {
            Entity = entity;
            Slot = inventoryChange.Slot;
            Item = inventoryChange.Item;
            Kind = inventoryChange.Kind;
        }
    }

    public class InventoryEquipmentModuleFactory : IEntityModuleFactory, IEntityModulePersistance<InventoryEquipmentModule, (InventorySave InventorySave, EquipmentSave EquipmentSave)>
    {
        private readonly InventoryModelAssembler _inventoryModelAssembler;
        private readonly EquipmentModelAssembler _equipmentModelAssembler;

        public InventoryEquipmentModuleFactory(InventoryModelAssembler inventoryModelAssembler, EquipmentModelAssembler equipmentModelAssembler)
        {
            _inventoryModelAssembler = inventoryModelAssembler;
            _equipmentModelAssembler = equipmentModelAssembler;
        }

        public EntityModuleBase Create(EntityDefinition entityDefinition)
        {
            InventoryModel inventoryModel = null;
            if (entityDefinition.TryGetModuleDefinition<InventoryModuleDefinition>(out var inventoryDefinitionModule))
            {
                inventoryModel = _inventoryModelAssembler.Create(inventoryDefinitionModule.SlotsAmount);
            }

            EquipmentModel equipmentModel = null;
            if (entityDefinition.TryGetModuleDefinition<EquipmentModuleDefinition>(out var equipmentDefinitionModule))
            {
                var slots = equipmentDefinitionModule.EquipmentSlots.ToArray();
                equipmentModel = _equipmentModelAssembler.Create(slots);
            }

            return new InventoryEquipmentModule(inventoryModel, equipmentModel);
        }

        public void Apply(InventoryEquipmentModule entityInventoryEquipmentModule, (InventorySave InventorySave, EquipmentSave EquipmentSave) entitySave)
        {
            _inventoryModelAssembler.Apply(entityInventoryEquipmentModule.Inventory, entitySave.InventorySave);
            _equipmentModelAssembler.Apply(entityInventoryEquipmentModule.Equipment, entitySave.EquipmentSave);
        }

        public (InventorySave InventorySave, EquipmentSave EquipmentSave) Save(InventoryEquipmentModule entityInventoryEquipmentModule)
        {
            var inventorySave = _inventoryModelAssembler.Save(entityInventoryEquipmentModule.Inventory);
            var equipmentSave = _equipmentModelAssembler.Save(entityInventoryEquipmentModule.Equipment);
            return (inventorySave, equipmentSave);
        }
    }
}
