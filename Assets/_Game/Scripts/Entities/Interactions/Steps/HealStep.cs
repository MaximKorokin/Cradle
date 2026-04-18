using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;

namespace Assets._Game.Scripts.Entities.Interactions.Steps
{
    public class HealStep : IInteractionStep
    {
        private readonly int _amount;
        private readonly IGlobalEventBus _globalEventBus;

        private bool _done;

        public HealStep(int amount, IGlobalEventBus globalEventBus) => (_amount, _globalEventBus) = (amount, globalEventBus);

        public void Start(in InteractionContext context) => _done = false;

        public StepStatus Tick(in InteractionContext context, float delta)
        {
            if (_done) return StepStatus.Completed;

            if (context.Target.TryGetModule(out HealthModule healthModule))
            {
                healthModule.Heal(_amount);
                _globalEventBus.Publish(new HealAppliedEvent(context.Target, context.Source, _amount));
            }

            _done = true;
            return StepStatus.Completed;
        }

        public void Cancel(in InteractionContext context) { }
    }

    public readonly struct HealAppliedEvent : IGlobalEvent
    {
        public readonly Entity Target;
        public readonly Entity Source;
        public readonly float Heal;

        public HealAppliedEvent(Entity target, Entity source, float heal)
        {
            Target = target;
            Source = source;
            Heal = heal;
        }
    }
}
