using Assets._Game.Scripts.Entities.Items.Equipment;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Items.Traits
{
    [Serializable]
    public class EquippableTrait : ItemTraitBase
    {
        [field: SerializeField]
        public EquipmentSlotType Slot { get; private set; }
        //public int RequiredLevel;
    }
}