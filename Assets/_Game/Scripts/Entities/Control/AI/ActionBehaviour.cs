using Assets._Game.Scripts.Entities.Control.Intents;
using Assets._Game.Scripts.Entities.Interactions.Action;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Shared.Extensions;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Control.AI
{
    public sealed class ActionBehaviour : IAiBehaviour
    {
        private readonly ActionEvaluator _actionEvaluator;

        public ActionBehaviour(ActionEvaluator actionEvaluator)
        {
            _actionEvaluator = actionEvaluator;
        }

        public BehaviourEvaluation Evaluate(Entity entity)
        {
            return _actionEvaluator.Evaluate(entity);
        }

        public void Tick(Entity entity, IBehaviourContext context, float delta)
        {
            var intentModule = entity.GetModule<IntentModule>();

            if (context is not ActionBehaviourContext actionContext || actionContext.ActionInstance == null)
            {
                intentModule.ClearMove();
                return;
            }

            var targetPosition = actionContext.Target.GetPosition();

            intentModule.SetAim(new(targetPosition));

            if (ApproachActionRange(entity, intentModule, targetPosition, actionContext))
            {
                intentModule.SetAct(new ActionIntent(actionContext.ActionInstance, actionContext.Target, targetPosition));
            }
        }

        /// <summary>
        /// Returns true if the entity is within preparation range, otherwise sets the move intent towards the target and returns false
        /// </summary>
        private bool ApproachActionRange(Entity entity, IntentModule intentModule, Vector2 targetPosition, ActionBehaviourContext context)
        {
            var direction = targetPosition - entity.GetPosition();

            var effectiveRange = context.ActionInstance.GetEffectiveRange(entity);
            if (direction.sqrMagnitude <= effectiveRange * effectiveRange)
            {
                intentModule.ClearMove();
                return true;
            }

            intentModule.SetMove(new(direction));
            return false;
        }
    }

    public sealed class ActionBehaviourContext : IBehaviourContext
    {
        public readonly ActionInstance ActionInstance;
        public readonly Entity Target;

        public ActionBehaviourContext(ActionInstance actionInstance, Entity target)
        {
            ActionInstance = actionInstance;
            Target = target;
        }
    }
}
