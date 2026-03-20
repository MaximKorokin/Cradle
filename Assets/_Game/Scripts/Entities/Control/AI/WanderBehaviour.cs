using Assets._Game.Scripts.Entities.Control.Intents;
using Assets._Game.Scripts.Entities.Modules;
using Assets.CoreScripts;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Control.AI
{
    public sealed class WanderBehaviour : IAiBehaviour
    {
        private const float StopDistance = 0.1f;

        private readonly CooldownCounter _wanderCooldown = new();

        public ActionEvaluation Evaluate(Entity entity)
        {
            return new ActionEvaluation(0.01f, default);
        }

        public void Tick(Entity entity, ActionContext context, float delta)
        {
            if (!_wanderCooldown.IsOver())
            {
                return;
            }

            var wander = entity.GetModule<WanderBehaviourModule>();
            if (wander.AnchorPoint == null) return;

            var spatial = entity.GetModule<SpatialModule>();
            var intent = entity.GetModule<IntentModule>();

            if (wander.CurrentPoint == null || (wander.CurrentPoint.Value - spatial.Position).sqrMagnitude < StopDistance * StopDistance)
            {
                wander.CurrentPoint = wander.AnchorPoint + Random.insideUnitCircle * 3f;
                intent.SetMove(default);
                ResetWanderCooldown(wander.MinIdleTime, wander.MaxIdleTime);
                return;
            }

            intent.SetMove(new MoveIntent(wander.CurrentPoint.Value - spatial.Position));
        }

        private void ResetWanderCooldown(float minIdleTime, float maxIdleTime)
        {
            _wanderCooldown.Cooldown = Random.Range(minIdleTime, maxIdleTime);
            _wanderCooldown.Reset();
        }
    }
}
