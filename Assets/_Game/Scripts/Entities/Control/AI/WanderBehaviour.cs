using Assets._Game.Scripts.Entities.Control.Intents;
using Assets._Game.Scripts.Entities.Modules;
using Assets.CoreScripts;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Control.AI
{
    public sealed class WanderBehaviour : AiBehaviourBase
    {
        private const float StopDistance = 0.1f;
        private const float MinIdleTime = 3;
        private const float MaxIdleTime = 8;

        private Vector2 _initialPosition;

        private Vector2 _target;

        private CooldownCounter _wanderCooldown;

        protected override void OnInitialize()
        {
            _initialPosition = Entity.GetModule<SpatialModule>().Position;
            _wanderCooldown = new();
            ResetWanderCooldown();
        }

        public override float Evaluate()
        {
            return 0.1f;
        }

        public override void Execute(float delta)
        {
            var spatial = Entity.GetModule<SpatialModule>();
            var intent = Entity.GetModule<IntentModule>();

            if (_wanderCooldown.IsOver())
            {
                _target = _initialPosition + Random.insideUnitCircle * 3f;
            }

            var direction = _target - spatial.Position;

            if (direction.sqrMagnitude < StopDistance * StopDistance)
            {
                intent.SetMove(MoveIntent.None);
                ResetWanderCooldown();
                return;
            }

            intent.SetMove(new MoveIntent(direction));
        }

        private void ResetWanderCooldown()
        {
            _wanderCooldown.Cooldown = Random.Range(MinIdleTime, MaxIdleTime);
            _wanderCooldown.Reset();
        }
    }
}
