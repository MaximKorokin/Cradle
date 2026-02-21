using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public sealed class ComponentPool<T> where T : Component, IPoolable
{
    private readonly IObjectResolver _resolver;
    private readonly T _prefab;
    private readonly Transform _root;
    private readonly Stack<T> _stack = new();

    public ComponentPool(IObjectResolver resolver, T prefab, Transform root = null, int prewarm = 0)
    {
        _resolver = resolver;
        _prefab = prefab;
        _root = root;
        if (prewarm > 0) Prewarm(prewarm);
    }

    public void Prewarm(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var instance = _resolver.Instantiate(_prefab, _root);
            instance.Prefab = _prefab;
            instance.gameObject.SetActive(false);
            _stack.Push(instance);
        }
    }

    public T Take(Vector3 position, Quaternion rotation, Transform parent = null)
    {
        var obj = _stack.Count > 0 ? _stack.Pop() : Object.Instantiate(_prefab);
        var transform = obj.transform;

        transform.SetParent(parent != null ? parent : _root, false);
        transform.SetPositionAndRotation(position, rotation);

        obj.gameObject.SetActive(true);
        if (obj is IPoolable p) p.OnTake();

        return obj;
    }

    public void Return(T obj)
    {
        if (obj == null) return;

        if (obj is IPoolable p) p.OnReturn();
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(_root, worldPositionStays: false);

        _stack.Push(obj);
    }
}
