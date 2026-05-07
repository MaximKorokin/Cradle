using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Traits;
using System.Linq;

namespace Assets._Game.Scripts.Shared.Extensions
{
    public static class EquipmentModelExtensions
    {
        /// <summary>
        /// Finds the first occupied equipment slot that matches the item's equipment slot type.
        /// Returns null if the item is not equippable or no matching slot is occupied.
        /// </summary>
        public static EquipmentSlotKey? FindOccupiedSlotForItem(this EquipmentModel equipmentModel, ItemStackSnapshot item)
        {
            return FindOccupiedSlotForItem(equipmentModel, item.Definition);
        }

        /// <summary>
        /// Finds the first occupied equipment slot that matches the item definition's equipment slot type.
        /// Returns null if the item definition is not equippable or no matching slot is occupied.
        /// </summary>
        public static EquipmentSlotKey? FindOccupiedSlotForItem(this EquipmentModel equipmentModel, ItemDefinition itemDefinition)
        {
            if (!itemDefinition.TryGetTrait<EquippableTrait>(out var equippableTrait))
                return null;

            var equipmentSlotType = equippableTrait.Slot;
            var occupiedSlot = equipmentModel.Enumerate()
                .FirstOrDefault(x => x.Slot.SlotType == equipmentSlotType && x.Snapshot != null).Slot;

            return occupiedSlot;
        }
    }
}
