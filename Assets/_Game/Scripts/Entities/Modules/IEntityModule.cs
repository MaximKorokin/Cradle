using System;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Entities.Modules
{
    public interface IEntityModule : IDisposable
    {
        void Attach(Entity entity);
        void Detach();
        void Initialize();
    }

    public abstract class EntityModuleBase : IEntityModule
    {
        private readonly Queue<Action> _pendingPublishes = new();

        public Entity Entity { get; private set; }

        public void Attach(Entity entity)
        {
            Entity = entity;

            if (Entity.IsCreated) OnEntityCreated(new());
            else Entity.SubscribeOnce<EntityCreatedEvent>(OnEntityCreated);

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

        protected virtual void OnEntityCreated(EntityCreatedEvent e)
        {
            while (_pendingPublishes.Count > 0)
            {
                _pendingPublishes.Dequeue().Invoke();
            }
        }

        protected void Publish<TEvent>(TEvent evt) where TEvent : struct, IEntityEvent
        {
            if (Entity != null)
            {
                Entity.Publish(evt);
            }
            else
            {
                _pendingPublishes.Enqueue(() => Entity.Publish(evt));
            }
        }

        public virtual void Initialize() { }

        public virtual void Dispose() { }
    }
}
