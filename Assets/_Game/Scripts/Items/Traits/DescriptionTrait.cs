using System;
using UnityEngine;

namespace Assets._Game.Scripts.Items.Traits
{
    [Serializable]
    public sealed class DescriptionTrait : ItemTraitBase
    {
        [field: SerializeField, TextArea]
        public string Description { get; private set; }
    }
}
