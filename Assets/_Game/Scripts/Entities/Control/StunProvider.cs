using Assets._Game.Scripts.Entities.Modules;

namespace Assets._Game.Scripts.Entities.Control
{
    public sealed class StunProvider : IControlProvider
    {
        public ControlPriority Priority => ControlPriority.OverrideHigh;
        public bool IsActive => _timeLeft > 0f;

        private float _timeLeft;

        public void Start(float duration) => _timeLeft = duration;

        public void Tick(Entity entity, float delta)
        {
            _timeLeft -= delta;

            if (entity.TryGetModule(out IntentModule intent))
                intent.StopMove();
        }
    }
}
