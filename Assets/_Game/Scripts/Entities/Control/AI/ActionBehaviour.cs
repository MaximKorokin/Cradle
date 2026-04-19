using Assets._Game.Scripts.Entities.Control.Intents;
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

        public ActionEvaluation Evaluate(Entity entity)
        {
            return _actionEvaluator.Evaluate(entity);
        }

        public void Tick(Entity entity, ActionContext context, float delta)
        {
            var intentModule = entity.GetModule<IntentModule>();

            if (context.ActionInstance == null)
            {
                intentModule.ClearMove();
                return;
            }

            var targetPosition = context.Target.GetPosition();

            intentModule.SetAim(new(targetPosition));

            if (ApproachActionRange(entity, intentModule, targetPosition, context))
            {
                intentModule.SetAct(new ActionIntent(context.ActionInstance, context.Target, targetPosition));
            }
        }

        /// <summary>
        /// Returns true if the entity is within preparation range, otherwise sets the move intent towards the target and returns false
        /// </summary>
        private bool ApproachActionRange(Entity entity, IntentModule intentModule, Vector2 targetPosition, ActionContext context)
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
}
