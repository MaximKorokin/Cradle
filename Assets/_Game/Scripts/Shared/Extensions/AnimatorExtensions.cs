using System;
using UnityEngine;

namespace Assets._Game.Scripts.Shared.Extensions
{
    public static class AnimatorExtensions
    {
        public static void SetAnimatorValue<T>(this Animator animator, string key, T value = default) where T : struct
        {
            if (animator == null || animator.runtimeAnimatorController == null)
            {
                return;
            }

            var keyName = key.ToString();
            switch (Array.Find(animator.parameters, x => x.name == keyName)?.type)
            {
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(keyName, Convert.ToBoolean(value));
                    break;
                case AnimatorControllerParameterType.Trigger:
                    animator.SetTrigger(keyName);
                    break;
                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(keyName, Convert.ToInt32(value));
                    break;
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(keyName, Convert.ToSingle(value));
                    break;
            }
        }
    }
}
