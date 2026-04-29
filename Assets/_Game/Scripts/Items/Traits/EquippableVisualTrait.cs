using Assets._Game.Scripts.Items.Equipment;
using UnityEngine;

namespace Assets._Game.Scripts.Items.Traits
{
    public sealed class EquippableVisualTrait : ItemTraitBase
    {
        [field: SerializeField]
        public EquipmentSlotType[] Slots { get; private set; }

        // The visual rule determines how the slots will change
        [field: SerializeField]
        public EquipVisualRule Rule { get; private set; }
    }

    public enum EquipVisualRule
    {
        // Following the standard visual rules for the slot
        Default = 0,
        // Duplicate the visuals of the item in the specified slots
        Mirror = 10,
        // Hide visuals in specified slots, regardless of the item's actual slot occupation
        Hide = 20
    }
}
