namespace Assets._Game.Scripts.Entities.Control
{
    public abstract class TimedControlProvider : ControlProviderBase
    {
        public override bool IsActive => _timeLeft > 0f;

        private float _timeLeft;

        protected TimedControlProvider(float duration)
        {
            _timeLeft = duration;
        }

        public override void Tick(float delta)
        {
            _timeLeft -= delta;
            if (IsActive) OnTick(delta);
            else OnComplete();
        }

        protected abstract void OnTick(float delta);
        protected abstract void OnComplete();
    }
}
