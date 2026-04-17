using System;

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
        public Entity Entity { get; private set; }

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

        protected void Publish<TEvent>(TEvent evt) where TEvent : struct, IEntityEvent
            => Entity.Publish(evt);

        public virtual void Initialize() { }

        public virtual void Dispose() { }
    }
}
