namespace Assets._Game.Scripts.Entities.Control
{
    public abstract class TimedControlProvider : IControlProvider
    {
        public abstract ControlPriority Priority { get; }
        public abstract ControlMask Mask { get; }
        public bool IsActive => _timeLeft > 0f;

        private float _timeLeft;

        protected TimedControlProvider(float duration)
        {
            _timeLeft = duration;
        }

        public void Tick(Entity e, float dt)
        {
            _timeLeft -= dt;
            OnTick(e, dt);
        }

        protected abstract void OnTick(Entity e, float dt);
    }
}
