using Assets._Game.Scripts.Entities.Units;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.Items.Traits
{
    [Serializable]
    public class AnimationOverrideTrait : ItemTraitBase
    {
        [field: SerializeField]
        public AnimationOverrideProfile AnimationOverrideProfile { get; private set; }
    }
}
