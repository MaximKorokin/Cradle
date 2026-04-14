using Assets._Game.Scripts.Infrastructure.Game;
using System.Collections.Generic;
using System;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public abstract class SystemBase : ISystem, IDisposable
    {
        protected readonly IGlobalEventBus GlobalEventBus;

        private readonly List<IDisposable> _subscriptions = new();

        protected SystemBase(IGlobalEventBus globalEventBus)
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

    public interface ISystem { }
}
