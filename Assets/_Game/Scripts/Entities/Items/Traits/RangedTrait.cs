using System;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Items.Traits
{
    [Serializable]
    public class RangedTrait : ItemTraitBase
    {
        [field: SerializeField]
        public float Damage { get; set; }
        [field: SerializeField]
        public float AttackSpeed { get; set; }
    }
}
