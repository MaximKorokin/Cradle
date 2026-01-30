using Assets._Game.Scripts.Shared.Extensions;
using Assets.CoreScripts;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Units
{
    public class UnitsAnimator
    {
        private readonly Animator _animator;

        public UnitsAnimator(Animator animator)
        {
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

        public void SetAnimation(EntityAnimationClipName key, AnimationClip animation)
        {
            if (_animator.runtimeAnimatorController is AnimatorOverrideController overrideController)
            {
                overrideController[key.ToString()] = animation;
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
