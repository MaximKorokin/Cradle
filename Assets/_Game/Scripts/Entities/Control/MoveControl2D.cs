using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Stats;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Control
{
    public sealed class MoveControl2D : MonoBehaviour, IEntityBindable
    {
        [SerializeField] private Rigidbody2D _rigidbody;

        private Entity _entity;

        public void Bind(Entity entity)
        {
            _entity = entity;
        }

        public void Unbind()
        {
            _entity = null;
            if (_rigidbody) _rigidbody.linearVelocity = Vector2.zero;
        }

        private void FixedUpdate()
        {
            if (_entity == null) return;
            SynchronizeSpatial();

            if (!_entity.TryGetModule(out IntentModule intent)) return;
            if (!_entity.TryGetModule(out StatModule stats)) return;

            float moveSpeed = stats.Stats.Get(StatId.MoveSpeed);
            var direction = intent.Move.NormalizedDirection;
            var multiplier = intent.Move.SpeedMultiplier;

            _rigidbody.linearVelocity = moveSpeed * multiplier * direction;
        }

        private void SynchronizeSpatial()
        {
            if (_entity.TryGetModule(out SpatialModule spatial))
                spatial.SetPosition(_rigidbody.linearVelocity);

            if (_entity.TryGetModule(out KinematicsModule kin))
                kin.SetVelocity(_rigidbody.linearVelocity);
        }
    }
}
