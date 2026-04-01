namespace Assets._Game.Scripts.Entities.Control
{
    public abstract class ControlProviderBase : IControlProvider
    {
        public abstract ControlPriority Priority { get; }
        public abstract ControlMask Mask { get; }
        public abstract bool IsActive { get; }

        protected Entity Entity { get; private set; }

        public virtual void Initialize(Entity entity)
        {
            Entity = entity;
        }

        public abstract void Tick(float delta);

        public virtual void Reset()
        {

        }
    }
}
