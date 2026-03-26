using UnityEngine;

namespace Assets._Game.Scripts.Items.Traits
{
    public sealed class UsableTrait : ItemTraitBase
    {
        [field: SerializeField]
        public bool Consumable { get; private set; }
        [field: SerializeField]
        public float Cooldown { get; private set; }
    }
}
