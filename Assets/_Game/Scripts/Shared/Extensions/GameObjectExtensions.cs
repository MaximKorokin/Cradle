using UnityEngine;

namespace Assets._Game.Scripts.Shared.Extensions
{
    public static class GameObjectExtensions
    {
        public static bool TryGetComponentInParent<T>(this GameObject gameObject, out T component)
        {
            component = gameObject == null ? default : gameObject.GetComponentInParent<T>();
            return component != null;
        }
    }
}
