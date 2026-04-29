using Assets._Game.Scripts.Items.Equipment;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.Items.Traits
{
    [Serializable]
    public class EquippableTrait : ItemTraitBase
    {
        // The primary slot is the main slot that this item occupies when equipped
        [field: SerializeField]
        public EquipmentSlotType Slot { get; private set; }

        // Secondary slots are optional slots that this item occupy, allowing for items that take up multiple slots (e.g. two-handed weapon, knuckles)
        [field: SerializeField]
        public EquipmentSlotType[] SecondarySlots { get; private set; }
    }
}
