using System;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Items.Traits
{
    [Serializable]
    public class HealTrait : ItemTraitBase
    {
        [field: SerializeField]
        public float HealAmount { get; set; }
    }
}
