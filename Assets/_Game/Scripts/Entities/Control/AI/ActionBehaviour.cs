using Assets._Game.Scripts.Entities.Control.Intents;
using Assets._Game.Scripts.Entities.Modules;

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
            if (context.ActionInstance == null)
                return;

            var targetPosition = context.Target.GetModule<SpatialModule>().Position;
            var intent = entity.GetModule<IntentModule>();

            intent.SetAct(new ActionIntent(context.ActionInstance, context.Target, targetPosition));
        }
    }
}
