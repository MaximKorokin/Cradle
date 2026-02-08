using Assets._Game.Scripts.Shared.Extensions;
using Assets.CoreScripts;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Units
{
    public class UnitsAnimator
    {
        private readonly Animator _animator;
        private readonly List<KeyValuePair<AnimationClip, AnimationClip>> _animatorOverrides = new();

        public UnitsAnimator(Animator animator, AnimatorOverrideController animatorController)
        {
            animatorController.GetOverrides(_animatorOverrides);
            var wrapper = new AnimatorOverrideController(animatorController);
            wrapper.ApplyOverrides(_animatorOverrides);
            animator.runtimeAnimatorController = wrapper;
            _animator = animator;
        }

        public void Rebind()
        {
            _animator.Rebind();
            _animator.Update(0f);
        }

        public void SetValue<T>(EntityAnimatorParameterName key, T value = default) where T : struct
        {
            _animator.SetAnimatorValue(key.ToString(), value);
        }

        /// <summary>
        /// Overrides an animation. If clip is null will return to default clip
        /// </summary>
        public void SetAnimation(EntityAnimationClipName key, AnimationClip animation)
        {
            var overrideAnimation = animation;
            if (animation == null)
            {
                overrideAnimation = _animatorOverrides.FirstOrDefault(x => x.Key.name == key.ToString()).Value;
            }
            if (_animator.runtimeAnimatorController is AnimatorOverrideController overrideController)
            {
                overrideController[key.ToString()] = overrideAnimation;
            }
            else
            {
                SLog.Error($"{nameof(_animator.runtimeAnimatorController)} is expected to be of type {nameof(AnimatorOverrideController)}");
            }
        }
    }

    public enum EntityAnimationClipName
    {
        Empty = 0,
        IdleBody = 10,
        IdleHands = 11,
        IdleFeet = 12,
        WalkBody = 20,
        WalkHands = 21,
        WalkFeet = 22,
        ActionBody = 30,
        ActionHands = 31,
        Die = 100,
    }

    public enum EntityAnimatorParameterName
    {
        ToDie = 10,
        ToAction = 20,
        ToIdle = 30,
        IsWalking = 40,
    }
}
