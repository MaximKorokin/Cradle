using Assets._Game.Scripts.Entities.Modules;
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

            if (!_entity.TryGetModule(out KinematicsModule kinematics)) return;

            _rigidbody.linearVelocity = kinematics.Velocity;
        }
    }
}
