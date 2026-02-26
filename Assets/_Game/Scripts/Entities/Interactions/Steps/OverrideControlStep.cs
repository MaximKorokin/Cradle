using Assets._Game.Scripts.Entities.Control;
using Assets._Game.Scripts.Entities.Modules;

namespace Assets._Game.Scripts.Entities.Interactions.Steps
{
    public sealed class OverrideControlStep : IInteractionStep
    {
        private readonly IControlProvider _controlProvider;
        private bool _done;

        public OverrideControlStep(IControlProvider controlProvider)
        {
            _controlProvider = controlProvider;
        }

        public void Start(in InteractionContext ctx) => _done = false;

        public StepStatus Tick(in InteractionContext context, float delta)
        {
            if (_done) return StepStatus.Completed;

            context.Target.Publish(new OverrideControlRequestEvent(_controlProvider));

            _done = true;
            return StepStatus.Completed;
        }

        public void Cancel(in InteractionContext ctx) { }
    }
}
