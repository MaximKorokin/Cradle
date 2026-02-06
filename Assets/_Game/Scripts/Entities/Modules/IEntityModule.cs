using System;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Entities.Modules
{
    public interface IEntityModule : IDisposable
    {
        void Attach(Entity entity);
        void Detach();
    }

    public abstract class EntityModuleBase : IEntityModule
    {
        private readonly List<IDisposable> _subscriptions = new();

        protected Entity Entity { get; private set; }

        public void Attach(Entity entity)
        {
            Entity = entity;
            OnAttach();
        }

        public void Detach()
        {
            Dispose();
            Entity = null!;
            OnDetach();
        }

        protected virtual void OnAttach() { }
        protected virtual void OnDetach() { }

        protected void Track(IDisposable subscription) => _subscriptions.Add(subscription);

        protected void Subscribe<TEvent>(Action<TEvent> action) where TEvent : struct, IEntityEvent
        {
            Track(Entity.Subscribe(action));
        }

        protected bool TryGet<T>(out T module) where T : class, IEntityModule
            => Entity.TryGetModule(out module);

        protected void Publish<TEvent>(TEvent evt) where TEvent : struct, IEntityEvent
            => Entity.Publish(evt);

        public virtual void Dispose()
        {
            for (int i = _subscriptions.Count - 1; i >= 0; i--)
                _subscriptions[i].Dispose();
            _subscriptions.Clear();
        }
    }
}
