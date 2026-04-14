using Assets._Game.Scripts.Infrastructure.Game;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Systems
{
    public abstract class UISystemBase : MonoBehaviour, IDisposable
    {
        protected IGlobalEventBus GlobalEventBus;

        private readonly List<IDisposable> _subscriptions = new();

        protected virtual void BaseConstruct(IGlobalEventBus globalEventBus)
        {
            GlobalEventBus = globalEventBus;
        }

        protected void TrackGlobalEvent<T>(Action<T> handler)
            where T : struct, IGlobalEvent
        {
            var subscription = GlobalEventBus.Subscribe(handler);
            _subscriptions.Add(subscription);
        }

        public virtual void Dispose()
        {
            for (int i = 0; i < _subscriptions.Count; i++)
            {
                _subscriptions[i].Dispose();
            }
            _subscriptions.Clear();
        }
    }
}
