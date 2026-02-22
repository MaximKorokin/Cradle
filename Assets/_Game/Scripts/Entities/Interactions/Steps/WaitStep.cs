namespace Assets._Game.Scripts.Entities.Interactions.Steps
{
    public sealed class WaitStep : IInteractionStep
    {
        private readonly float _seconds;
        private float _timeLeft;

        public WaitStep(float seconds) => _seconds = seconds;

        public void Start(in InteractionContext context) => _timeLeft = _seconds;

        public StepStatus Tick(in InteractionContext context, float delta)
        {
            _timeLeft -= delta;
            return _timeLeft > 0f ? StepStatus.Running : StepStatus.Completed;
        }

        public void Cancel(in InteractionContext context) { }
    }
}
