using Assets._Game.Scripts.Entities.Interactions.Steps;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class AttackModifierSystem : SystemBase
    {
        private readonly IGlobalEventBus _globalEventBus;

        public AttackModifierSystem(IGlobalEventBus globalEventBus)
        {
            _globalEventBus = globalEventBus;

            _globalEventBus.Subscribe<DamageAppliedEvent>(OnDamageApplied);
        }

        public override void Dispose()
        {
            base.Dispose();

            _globalEventBus.Subscribe<DamageAppliedEvent>(OnDamageApplied);
        }

        private void OnDamageApplied(DamageAppliedEvent e)
        {

        }
    }
}
