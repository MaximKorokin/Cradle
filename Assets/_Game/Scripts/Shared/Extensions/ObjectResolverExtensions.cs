using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets._Game.Scripts.Shared.Extensions
{
    public static class ObjectResolverExtensions
    {
        public static void InstantiateFromPrefab<T>(this IObjectResolver resolver, T prefab, ref T instance) where T : Component
        {
            if (instance == null)
            {
                instance = resolver.Instantiate(prefab);
                instance.gameObject.SetActive(false);
            }
        }
    }
}
