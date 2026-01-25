using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.CoreScripts
{
    public abstract class MonoBehaviourBase : MonoBehaviour
    {
        public event Action OnDestroying;

        protected virtual void Awake()
        {

        }

        protected virtual void Start()
        {

        }

        protected virtual void OnEnable()
        {

        }

        protected virtual void OnDisable()
        {

        }

        protected virtual void Update()
        {

        }

        protected virtual void FixedUpdate()
        {

        }

        protected virtual void OnDestroy()
        {
            OnDestroying?.Invoke();
        }

        public virtual T GetRequiredComponent<T>() where T : Component
        {
            return gameObject.GetRequiredComponent<T>();
        }

        public virtual T GetRequiredComponentInChildren<T>() where T : Component
        {
            return gameObject.GetRequiredComponentInChildren<T>();
        }

        public virtual T GetRequiredComponentInParent<T>() where T : Component
        {
            return gameObject.GetRequiredComponentInParent<T>();
        }

        public virtual T GetRequiredComponentOrInChildren<T>() where T : Component
        {
            return gameObject.GetRequiredComponentOrInChildren<T>();
        }

        private readonly Dictionary<Delegate, object> _lazyCache = new();

        protected T GetLazy<T>(Func<T> factory)
        {
            return (T)_lazyCache.GetOrAdd(factory, () => factory());
        }
    }
}