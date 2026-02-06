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

            if (Equipment != null) Equipment.SlotChanged += OnEquipmentSlotChanged;
        }

        public InventoryModel Inventory { get; private set; }
        public EquipmentModel Equipment { get; private set; }

        private void OnEquipmentSlotChanged(EquipmentSlotKey slot)
        {
            Publish(new EquipmentChanged(slot));
        }

        public override void Dispose()
        {
            base.Dispose();
            if (Equipment != null) Equipment.SlotChanged -= OnEquipmentSlotChanged;
        }
    }

    public readonly struct EquipmentChanged : IEntityEvent
    {
        public readonly EquipmentSlotKey Slot;
        public EquipmentChanged(EquipmentSlotKey slot) => Slot = slot;
    }
}
