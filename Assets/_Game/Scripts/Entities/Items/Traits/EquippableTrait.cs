using Assets._Game.Scripts.Entities.Items.Equipment;
using System;
using System.Collections;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Items.Traits
{
    [Serializable]
    public class EquippableTrait : ItemTraitBase
    {
        public EquipmentSlotType Slot;
        //public int RequiredLevel;
    }
}