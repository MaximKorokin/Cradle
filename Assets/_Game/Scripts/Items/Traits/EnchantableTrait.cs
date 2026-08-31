using Assets._Game.Scripts.Items.Enchanting;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.Items.Traits
{
    [Serializable]
    public class EnchantableTrait : ItemTraitBase
    {
        [field: SerializeField]
        public ItemEnchantingDefinition ItemEnchantingDefinition { get; private set; }
    }
}
