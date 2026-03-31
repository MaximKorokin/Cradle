using Assets._Game.Scripts.Entities.Modules;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Control
{
    public sealed class MoveControl2D : MonoBehaviour, IEntityBindable
    {
        [SerializeField] private Rigidbody2D _rigidbody;

        private KinematicsModule _kinematicsModule;

        public void Bind(Entity entity)
        {
            _kinematicsModule = entity.GetModule<KinematicsModule>();
        }

        public void Unbind()
        {
            _kinematicsModule = null;
            if (_rigidbody != null) _rigidbody.linearVelocity = Vector2.zero;
        }

        private void FixedUpdate()
        {
            if (_kinematicsModule != null)
            {
                _rigidbody.linearVelocity = _kinematicsModule.Velocity;
            }
        }
    }
}
