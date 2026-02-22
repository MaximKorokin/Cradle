namespace Assets._Game.Scripts.Entities.Interaction.Steps
{
    public sealed class SequenceStep : IInteractionStep
    {
        private readonly IInteractionStep[] _steps;
        private int _currentStepIndex;

        public SequenceStep(params IInteractionStep[] steps) => _steps = steps;

        public void Start(in InteractionContext context)
        {
            _currentStepIndex = 0;
            if (_steps.Length > 0) _steps[0].Start(context);
        }

        public StepStatus Tick(in InteractionContext context, float delta)
        {
            while (_currentStepIndex < _steps.Length)
            {
                var stepStatus = _steps[_currentStepIndex].Tick(context, delta);
                if (stepStatus == StepStatus.Running) return StepStatus.Running;
                if (stepStatus == StepStatus.Failed) return StepStatus.Failed;

                // Current step completed, move to the next one
                _steps[_currentStepIndex].Cancel(context);
                _currentStepIndex++;
                if (_currentStepIndex < _steps.Length)
                {
                    _steps[_currentStepIndex].Start(context);
                }
            }
            return StepStatus.Completed;
        }

        public void Cancel(in InteractionContext context)
        {
            if (_currentStepIndex < _steps.Length)
            {
                _steps[_currentStepIndex].Cancel(context);
            }
        }
    }
}