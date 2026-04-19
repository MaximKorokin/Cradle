using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;

namespace Assets._Game.Scripts.Entities.Interactions.Steps
{
    public sealed class ReviveStep : IInteractionStep
    {
        private readonly IGlobalEventBus _globalEventBus;

        private bool _done;

        public ReviveStep(IGlobalEventBus globalEventBus)
        {
            _globalEventBus = globalEventBus;
        }

        public void Start(in InteractionContext context)
        {
            _done = false;
        }

        public StepStatus Tick(in InteractionContext context, float delta)
        {
            if (_done) return StepStatus.Completed;
            _globalEventBus.Publish(new EntityReviveRequest(context.Target, true));
            _done = true;
            return StepStatus.Completed;
        }

        public void Cancel(in InteractionContext context)
        {

        }
    }
}
