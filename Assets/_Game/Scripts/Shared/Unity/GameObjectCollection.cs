using System.Collections.Generic;
using UnityEngine;

namespace Assets._Game.Scripts.Shared.Unity
{
    public sealed class GameObjectCollection
    {
        private readonly HashSet<GameObject> _objects = new();

        public IReadOnlyCollection<GameObject> Objects => _objects;

        public T Instantiate<T>(
            T prefab,
            Transform parent = null,
            bool worldPositionStays = false)
            where T : Component
        {
            var instance = Object.Instantiate(prefab, parent, worldPositionStays);
            Track(instance.gameObject);
            return instance;
        }

        public GameObject Instantiate(
            GameObject prefab,
            Transform parent = null,
            bool worldPositionStays = false)
        {
            var instance = Object.Instantiate(prefab, parent, worldPositionStays);
            Track(instance);
            return instance;
        }

        public T Track<T>(T instance) where T : Component
        {
            if (instance != null)
                _objects.Add(instance.gameObject);

            return instance;
        }

        public GameObject Track(GameObject instance)
        {
            if (instance != null)
                _objects.Add(instance);

            return instance;
        }

        public bool Untrack(GameObject instance)
        {
            if (instance != null)
                return _objects.Remove(instance);
            return false;
        }

        public void Clear()
        {
            foreach (var instance in _objects)
            {
                if (instance != null)
                    Object.Destroy(instance);
            }

            _objects.Clear();
        }
    }
}
