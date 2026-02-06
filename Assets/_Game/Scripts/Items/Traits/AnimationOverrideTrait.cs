using Assets._Game.Scripts.Entities.Units;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.Items.Traits
{
    [Serializable]
    public class AnimationOverrideTrait : ItemTraitBase
    {
        [field: SerializeField]
        public EntityAnimationClipName AnimationKey { get; private set; }
        [field: SerializeField]
        public AnimationClip AnimationClip { get; private set; }
    }
}
