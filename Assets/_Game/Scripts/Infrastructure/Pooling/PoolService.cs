using Assets.CoreScripts;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public sealed class PoolService
{
    private readonly IObjectResolver _resolver;

    private readonly Dictionary<Component, object> _pools = new();
    private readonly Transform _root;

    public PoolService(IObjectResolver resolver)
    {
        _resolver = resolver;
        _root = new GameObject("Pools").transform;
    }

    public ComponentPool<T> GetOrCreate<T>(T prefab, int prewarm = 0) where T : Component, IPoolable
    {
        if (_pools.TryGetValue(prefab, out var poolObject))
        {
            return (ComponentPool<T>)poolObject;
        }

        var poolRoot = new GameObject($"Pool_{prefab.name}").transform;
        poolRoot.SetParent(_root, false);
        var pool = new ComponentPool<T>(_resolver, prefab, poolRoot, prewarm);
        _pools[prefab] = pool;

        return pool;
    }

    public T Take<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent = null) where T : Component, IPoolable
    {
        var pool = GetOrCreate(prefab);
        return pool.Take(position, rotation, parent);
    }

    public void Return<T>(T instance) where T : Component, IPoolable
    {
        if (!_pools.TryGetValue(instance.Prefab, out var poolObject))
        {
            SLog.Warn($"No pool found for prefab {instance.Prefab.name}. Destroying instance.");
            Object.Destroy(instance.gameObject);
            return;
        }

        if (poolObject is ComponentPool<T> pool)
        {
            pool.Return(instance);
        }
        else
        {
            SLog.Warn($"Pool type mismatch for prefab {instance.Prefab.name}. Expected {typeof(T)}, found {poolObject.GetType()}. Destroying instance.");
            Object.Destroy(instance.gameObject);
        }
    }
}
