using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Querying;
using Assets._Game.Scripts.UI.Systems;

namespace Assets._Game.Scripts.Entities.Control.AI
{
    public sealed class InteractionBehaviour : IAiBehaviour
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly IEntitySensor _entitySensor;
        private bool _isPromptShowing;

        public InteractionBehaviour(IGlobalEventBus globalEventBus, IEntitySensor entitySensor)
        {
            _globalEventBus = globalEventBus;
            _entitySensor = entitySensor;
        }

        public BehaviourEvaluation Evaluate(Entity entity)
        {
            if (entity.TryGetModule<InteractionBehaviourModule>(out var module) &&
                _entitySensor.TryGetFirstInRange(entity, module.Radius, Faction.FactionRelation.Ally, default, out var foundEntity))
            {
                return new BehaviourEvaluation(1, new TargetBehaviourContext(foundEntity));
            }

            // Not found any target, hide the prompt if it was showing
            Reset();

            return new BehaviourEvaluation(0, null);
        }

        public void Reset()
        {
            if (_isPromptShowing)
            {
                _isPromptShowing = false;
                _globalEventBus.Publish(InteractionPromptViewRequest.HideRequest());
            }
        }

        public void Tick(Entity entity, IBehaviourContext context, float delta)
        {
            if (entity.TryGetModule<InteractionBehaviourModule>(out var module) && context is TargetBehaviourContext targetContext)
            {
                _isPromptShowing = true;
                _globalEventBus.Publish(InteractionPromptViewRequest.ShowRequest(
                    module.PromptText,
                    module.ButtonText,
                    () => module.Open(entity.Id, targetContext.Target.Id)));
            }
        }
    }
}
