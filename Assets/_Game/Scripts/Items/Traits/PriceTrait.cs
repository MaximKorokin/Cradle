using System;
using UnityEngine;

namespace Assets._Game.Scripts.Items.Traits
{
    [Serializable]
    public sealed class PriceTrait : ItemTraitBase
    {
        [field: SerializeField]
        public int BasePrice { get; private set; }
    }
}
