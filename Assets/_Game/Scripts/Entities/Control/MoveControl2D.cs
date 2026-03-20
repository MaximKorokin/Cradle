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

            if (!_entity.TryGetModule(out IntentModule intent)) return;
            if (!_entity.TryGetModule(out StatModule stats)) return;

            float moveSpeed = stats.Stats.Get(StatId.MoveSpeed);

            if (intent.TryConsumeMove(out var moveIntent))
            {
                var direction = moveIntent.NormalizedDirection;
                var multiplier = moveIntent.SpeedMultiplier;

                _rigidbody.linearVelocity = moveSpeed * multiplier * direction;
            }
            else
            {
                _rigidbody.linearVelocity = Vector2.zero;
            }
        }
    }
}
