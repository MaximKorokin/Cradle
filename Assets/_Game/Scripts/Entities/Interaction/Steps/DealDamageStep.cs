using Assets._Game.Scripts.Entities.Interaction.Calculators;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Stats;

namespace Assets._Game.Scripts.Entities.Interaction.Steps
{
    public sealed class DealDamageStep : IInteractionStep
    {
        private readonly DamageSpec _spec;
        private readonly IDamageCalculator _calculator;

        private bool _done;

        public DealDamageStep(DamageSpec spec, IDamageCalculator calc)
        {
            _spec = spec;
            _calculator = calc;
        }

        public void Start(in InteractionContext context) => _done = false;

        public StepStatus Tick(in InteractionContext context, float delta)
        {
            if (_done) return StepStatus.Completed;

            var damage = _calculator.Calculate(_spec, context.Source, context.Target);
            if (context.Target.TryGetModule(out StatModule stats))
            {
                var hp = stats.Stats.Get(StatId.HpCurrent);
                stats.SetBase(StatId.HpCurrent, hp - damage);
            }

            _done = true;
            return StepStatus.Completed;
        }

        public void Cancel(in InteractionContext context) { }
    }

}