using UnityEngine;

namespace Assets._Game.Scripts.Items.Traits
{
    public sealed class OrderInLayerOverrideTrait : ItemTraitBase
    {
        [field: SerializeField]
        public OrderInLayerOverrideKind Kind { get; private set; }
    }

    public enum OrderInLayerOverrideKind
    {
        None = 0,
        Weapon = 10,
        Clothing = 20,
        Overlay = 30,
    }
}
