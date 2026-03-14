using Assets._Game.Scripts.Entities.Interactions;
using Assets._Game.Scripts.Entities.Interactions.Action;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Querying;

namespace Assets._Game.Scripts.Entities.Control.AI
{
    public sealed class ActionEvaluator
    {
        private readonly IEntitySensor _sensor;

        public ActionEvaluator(IEntitySensor entitySensor)
        {
            _sensor = entitySensor;
        }

        public ActionEvaluation Evaluate(Entity entity)
        {
            if (!entity.TryGetModule<ActionModule>(out var actionModule))
                return default;

            var bestEvaluation = default(ActionEvaluation);

            for (var i = 0; i < actionModule.Actions.Count; i++)
            {
                var action = actionModule.Actions[i];
                if (action == null)
                    continue;
                var evaluation = EvaluateAction(entity, action);
                if (evaluation.Score > bestEvaluation.Score)
                {
                    bestEvaluation = evaluation;
                }
            }
            return bestEvaluation;
        }

        private ActionEvaluation EvaluateAction(Entity entity, ActionInstance actionInstance)
        {
            var range = 3; // vision range
            var relation = actionInstance.Definition.FactionRelation;
            var entityQuery = new EntityQuery(actionInstance.Definition.EntityQueryData);

            if (!_sensor.TryGetNearestInRange(entity, range, relation, entityQuery, out var target))
                return default;

            if (!actionInstance.CanStartCast(new InteractionContext(entity, target, target.GetModule<SpatialModule>().Position)))
                return default;

            var score = actionInstance.Definition.BaseScore;
            return new ActionEvaluation(
                score,
                new ActionContext(actionInstance, target)
            );
        }
    }
}
