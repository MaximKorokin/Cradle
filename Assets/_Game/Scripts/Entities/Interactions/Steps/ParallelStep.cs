namespace Assets._Game.Scripts.Entities.Interactions.Steps
{
    public sealed class ParallelStep : IInteractionStep
    {
        private readonly IInteractionStep[] _steps;
        private bool[] _done;

        public ParallelStep(params IInteractionStep[] steps) => _steps = steps;

        public void Start(in InteractionContext context)
        {
            _done = new bool[_steps.Length];
            for (int i = 0; i < _steps.Length; i++)
            {
                _steps[i].Start(context);
            }
        }

        public StepStatus Tick(in InteractionContext context, float delta)
        {
            var allDone = true;
            for (int i = 0; i < _steps.Length; i++)
            {
                if (_done[i]) continue;

                var stepStatus = _steps[i].Tick(context, delta);
                if (stepStatus == StepStatus.Failed) return StepStatus.Failed;
                if (stepStatus == StepStatus.Completed) _done[i] = true;
                else allDone = false;
            }
            return allDone ? StepStatus.Completed : StepStatus.Running;
        }

        public void Cancel(in InteractionContext context)
        {
            for (int i = 0; i < _steps.Length; i++)
            {
                if (!_done[i]) _steps[i].Cancel(context);
            }
        }
    }
}