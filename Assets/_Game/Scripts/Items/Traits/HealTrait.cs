using System;
using UnityEngine;

namespace Assets._Game.Scripts.Items.Traits
{
    [Serializable]
    public class HealTrait : ItemTraitBase
    {
        [field: SerializeField]
        public float HealAmount { get; set; }
    }
}
