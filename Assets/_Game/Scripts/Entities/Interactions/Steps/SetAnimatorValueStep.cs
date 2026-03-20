using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Units;
using static Assets._Game.Scripts.Entities.Interactions.InteractionDefinition.SetAnimatorValueStepData;

namespace Assets._Game.Scripts.Entities.Interactions.Steps
{
    public class SetAnimatorValueStep : IInteractionStep
    {
        private readonly EntityAnimatorParameterName _parameter;
        private readonly int _value;
        private readonly ApplyToTarget _applyTo;

        public SetAnimatorValueStep(EntityAnimatorParameterName parameter, int value, ApplyToTarget applyTo)
        {
            _parameter = parameter;
            _value = value;
            _applyTo = applyTo;
        }

        public void Start(in InteractionContext context)
        {
            
        }

        public StepStatus Tick(in InteractionContext context, float delta)
        {
            Entity entity;
            if (_applyTo == ApplyToTarget.Source) entity = context.Source;
            else if (_applyTo == ApplyToTarget.Target) entity = context.Target;
            else return StepStatus.Failed;

            if (!entity.TryGetModule<AppearanceModule>(out var appearanceModule)) return StepStatus.Failed;

            appearanceModule.RequestSetAnimatorValue(_parameter, _value);

            return StepStatus.Completed;
        }

        public void Cancel(in InteractionContext context)
        {

        }
    }
}
