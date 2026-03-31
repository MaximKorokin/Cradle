using Assets._Game.Scripts.Entities.Modules;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Control
{
    public sealed class PositioningSynchronizer2D : MonoBehaviour, IEntityBindable
    {
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private Collider2D _collider;

        private SpatialModule _spatialModule;
        private RestrictionStateModule _restrictionStateModule;

        public void Bind(Entity entity)
        {
            _spatialModule = entity.GetModule<SpatialModule>();
            _restrictionStateModule = entity.GetModule<RestrictionStateModule>();

            if (_spatialModule != null)
                _spatialModule.SetPositionRequested += OnSetPositionRequested;

            SynchronizeSpatial();
        }

        private void OnSetPositionRequested(Vector2 position)
        {
            // immediate position change
            _rigidbody.position = position;
            _rigidbody.transform.position = position;
        }

        public void Unbind()
        {
            if (_spatialModule != null)
                _spatialModule.SetPositionRequested -= OnSetPositionRequested;
            
            _spatialModule = null;
            _restrictionStateModule = null;
        }

        private void FixedUpdate()
        {
            SynchronizeSpatial();
        }

        private void SynchronizeSpatial()
        {
            if (_spatialModule != null)
            {
                _spatialModule.SynchronizePosition(transform.position);
            }

            if (_restrictionStateModule != null)
            {
                _collider.enabled = !_restrictionStateModule.Has(RestrictionState.Disabled | RestrictionState.Dead);
            }
        }
    }
}
