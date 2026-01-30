using UnityEngine;

namespace Assets._Game.Scripts.Items.Traits
{
    public class MeleeTrait : ItemTraitBase
    {
        [field: SerializeField]
        public float Damage { get; set; }
        [field: SerializeField]
        public float AttackSpeed { get; set; }
    }
}
