using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Calculators;
using Assets._Game.Scripts.Infrastructure.Game;

namespace Assets._Game.Scripts.Entities.Interactions.Steps
{
    public sealed class DealDamageStep : IInteractionStep
    {
        private readonly DamageSpec _spec;
        private readonly IDamageCalculator _calculator;
        private readonly IGlobalEventBus _globalEventBus;

        private bool _done;

        public DealDamageStep(DamageSpec spec, IDamageCalculator calc, IGlobalEventBus globalEventBus)
        {
            _spec = spec;
            _calculator = calc;
            _globalEventBus = globalEventBus;
        }

        public void Start(in InteractionContext context) => _done = false;

        public StepStatus Tick(in InteractionContext context, float delta)
        {
            if (_done) return StepStatus.Completed;

            var damage = _calculator.Calculate(_spec, context.Source, context.Target, out bool isCritical);
            if (context.Target.TryGetModule(out HealthModule healthModule))
            {
                var appliedDamage = healthModule.ApplyDamage(damage);
                _globalEventBus.Publish(new DamageAppliedEvent(context.Target, context.Source, appliedDamage, isCritical, _spec.Source));
            }

            _done = true;
            return StepStatus.Completed;
        }

        public void Cancel(in InteractionContext context) { }
    }

    public readonly struct DamageAppliedEvent : IGlobalEvent
    {
        public readonly Entity Target;
        public readonly Entity Source;
        public readonly float Damage;
        public readonly bool IsCritical;
        public readonly DamageSourceType SourceType;

        public DamageAppliedEvent(Entity target, Entity source, float damage, bool isCritical, DamageSourceType sourceType)
        {
            Target = target;
            Source = source;
            Damage = damage;
            IsCritical = isCritical;
            SourceType = sourceType;
        }
    }

    public enum DamageSourceType
    {
        None = 0,
        Action = 10,
        AttackModifier = 20,
        Item = 30,
        Other = 100
    }
}
