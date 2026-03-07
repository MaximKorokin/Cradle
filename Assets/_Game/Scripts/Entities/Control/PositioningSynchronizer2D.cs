using Assets._Game.Scripts.Entities.Modules;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Control
{
    public sealed class PositioningSynchronizer2D : MonoBehaviour, IEntityBindable
    {
        [SerializeField] private Rigidbody2D _rigidbody;

        private Entity _entity;

        public void Bind(Entity entity)
        {
            _entity = entity;
            SynchronizeSpatial();
        }

        public void Unbind()
        {
            _entity = null;
            if (_rigidbody) _rigidbody.linearVelocity = Vector2.zero;
        }

        private void FixedUpdate()
        {
            SynchronizeSpatial();
        }

        private void SynchronizeSpatial()
        {
            if (_entity.TryGetModule(out SpatialModule spatial))
            {
                spatial.SetPosition(transform.position);
                spatial.SetFacing(_rigidbody.linearVelocity);
            }

            if (_entity.TryGetModule(out KinematicsModule kinematics))
            {
                kinematics.SetVelocity(_rigidbody.linearVelocity);
            }
        }
    }
}
