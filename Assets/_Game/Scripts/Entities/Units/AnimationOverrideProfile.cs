using System;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Units
{
    [CreateAssetMenu(fileName = "AnimationOverrideProfile", menuName = "Game/AnimationOverrideProfile")]
    public sealed class AnimationOverrideProfile : ScriptableObject
    {
        [SerializeField]
        private AnimationOverrideData[] _animationOverrides;
        public AnimationOverrideData[] AnimationOverrides => _animationOverrides;
    }

    [Serializable]
    public struct AnimationOverrideData
    {
        [SerializeField]
        private EntityAnimationClipName _animationKey;
        public readonly EntityAnimationClipName AnimationKey => _animationKey;

        [SerializeField]
        private AnimationClip _animationClip;
        public readonly AnimationClip AnimationClip => _animationClip;

        public AnimationOverrideData(EntityAnimationClipName animationKey, AnimationClip animationClip)
        {
            _animationKey = animationKey;
            _animationClip = animationClip;
        }
    }
}
