using Assets._Game.Scripts.Entities.Interactions;
using Assets._Game.Scripts.Entities.Interactions.Action;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Infrastructure.Querying;

namespace Assets._Game.Scripts.Entities.Control.AI
{
    public sealed class ActionEvaluator
    {
        private readonly ActionEvaluation _defaultActionEvaluation = new(float.NegativeInfinity, default);

        private readonly IEntitySensor _sensor;

        public ActionEvaluator(IEntitySensor entitySensor)
        {
            _sensor = entitySensor;
        }

        public ActionEvaluation Evaluate(Entity entity)
        {
            if (!entity.TryGetModule<ActionModule>(out var actionModule))
                return _defaultActionEvaluation;

            var bestEvaluation = _defaultActionEvaluation;

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

            if (entity.Definition.VariantName == "Player") SLog.Log($"Best behaviour: {bestEvaluation.Context.ActionInstance?.Definition.Name}");
            return bestEvaluation;
        }

        private ActionEvaluation EvaluateAction(Entity entity, ActionInstance actionInstance)
        {
            var range = 0f;
            if (entity.TryGetModule<StatModule>(out var statModule))
                range = statModule.Stats.Get(StatId.VisionRange);

            var relation = actionInstance.Definition.FactionRelation;
            var entityQuery = actionInstance.EntityQuery;

            if (!_sensor.TryGetNearestInRange(entity, range, relation, entityQuery, out var target, out var distance))
                return _defaultActionEvaluation;

            if (!actionInstance.CanStartPreparation(new InteractionContext(entity, target, target.GetModule<SpatialModule>().Position)))
                return _defaultActionEvaluation;

            var score = actionInstance.Definition.BaseScore - actionInstance.Definition.DistanceScorePenalty * distance;
            return new ActionEvaluation(
                score,
                new ActionContext(actionInstance, target)
            );
        }
    }
}
