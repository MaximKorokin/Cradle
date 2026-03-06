namespace Assets._Game.Scripts.Entities.Control
{
    public abstract class TimedControlProvider : IControlProvider
    {
        public abstract ControlPriority Priority { get; }
        public abstract ControlMask Mask { get; }
        public bool IsActive => _timeLeft > 0f;

        protected Entity Entity { get; private set; }

        private float _timeLeft;

        protected TimedControlProvider(float duration)
        {
            _timeLeft = duration;
        }

        public void Initialize(Entity entity)
        {
            Entity = entity;
        }

        public void Tick(float delta)
        {
            _timeLeft -= delta;
            OnTick(delta);
        }

        protected abstract void OnTick(float delta);
    }
}
