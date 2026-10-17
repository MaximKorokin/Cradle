using UnityEngine;

namespace Assets._Game.Scripts.Shared.Extensions
{
    public static class MonoBehaviourExtensions
    {
        public static bool TryGetComponentInParent<T>(this GameObject gameObject, out T component)
        {
            component = gameObject == null ? default : gameObject.GetComponentInParent<T>();
            return component != null;
        }

        public static bool TryGetComponentInParent<T>(this Behaviour behaviour, out T component)
        {
            component = behaviour == null ? default : behaviour.GetComponentInParent<T>();
            return component != null;
        }
    }
}
