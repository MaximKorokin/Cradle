using static Assets._Game.Scripts.Entities.Interactions.InteractionDefinition;

namespace Assets._Game.Scripts.Shared.Extensions
{
    public static class StepDataExtensions
    {
        public static bool TryGetStep<T>(this StepData step, out T value) where T : StepData
        {
            if (step is T typedStep)
            {
                value = typedStep;
                return true;
            }

            if (step is StepsStepData stepsStep)
            {
                for (int i = 0; i < stepsStep.Steps.Count; i++)
                {
                    if (stepsStep.Steps[i].TryGetStep(out value))
                    {
                        return true;
                    }
                }
            }

            value = null;
            return false;
        }
    }
}
