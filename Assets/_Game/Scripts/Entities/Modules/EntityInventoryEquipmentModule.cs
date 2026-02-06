using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;

namespace Assets._Game.Scripts.Entities.Modules
{
    public class EntityInventoryEquipmentModule : EntityModuleBase
    {
        public EntityInventoryEquipmentModule(InventoryModel inventory, EquipmentModel equipment)
        {
            Inventory = inventory;
            Equipment = equipment;

            if (Equipment != null) Equipment.EquipmentChanged += OnEquipmentSlotChanged;
        }

        public InventoryModel Inventory { get; private set; }
        public EquipmentModel Equipment { get; private set; }

        private void OnEquipmentSlotChanged(EquipmentChange equipmentChange)
        {
            Publish(new EquipmentChanged(equipmentChange));
        }

        public override void Dispose()
        {
            base.Dispose();
            if (Equipment != null) Equipment.EquipmentChanged -= OnEquipmentSlotChanged;
        }
    }

    public readonly struct EquipmentChanged : IEntityEvent
    {
        public readonly EquipmentSlotKey Slot;
        public readonly ItemStackSnapshot? Item;
        public readonly EquipmentChangeKind Kind;

        public EquipmentChanged(EquipmentSlotKey slot, ItemStackSnapshot? item, EquipmentChangeKind kind)
        {
            Slot = slot;
            Item = item;
            Kind = kind;
        }

        public EquipmentChanged(EquipmentChange equipmentChange)
        {
            Slot = equipmentChange.Slot;
            Item = equipmentChange.Item;
            Kind = equipmentChange.Kind;
        }
    }
}
