using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Stats;

namespace Assets._Game.Scripts.Entities.Interaction.Steps
{
    public class HealStep : IInteractionStep
    {
        private readonly int _amount;
        private bool _done;

        public HealStep(int amount) => _amount = amount;

        public void Start(in InteractionContext context) => _done = false;

        public StepStatus Tick(in InteractionContext context, float delta)
        {
            if (_done) return StepStatus.Completed;

            if (context.Target.TryGetModule(out StatModule stats))
            {
                var hp = stats.Stats.Get(StatId.HpCurrent);
                stats.SetBase(StatId.HpCurrent, hp - _amount);
            }

            _done = true;
            return StepStatus.Completed;
        }

        public void Cancel(in InteractionContext context) { }
    }
}
