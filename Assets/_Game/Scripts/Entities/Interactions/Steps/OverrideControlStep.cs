using Assets._Game.Scripts.Entities.Modules;

namespace Assets._Game.Scripts.Entities.Interactions.Steps
{
    public sealed class OverrideControlStep : IInteractionStep
    {
        private readonly float _duration;
        private bool _done;

        public OverrideControlStep(float duration)
        {
            _duration = duration;
        }

        public void Start(in InteractionContext ctx) => _done = false;

        public StepStatus Tick(in InteractionContext context, float delta)
        {
            if (_done) return StepStatus.Completed;
            context.Target.Publish(new OverrideControlRequestEvent(context.Source, context.Target, _duration));
            _done = true;
            return StepStatus.Completed;
        }

        public void Cancel(in InteractionContext ctx) { }
    }
}
