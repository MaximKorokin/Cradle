using Assets._Game.Scripts.Entities;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;

namespace Assets._Game.Scripts.Infrastructure.Game
{
    public interface ICameraService
    {
        Camera Camera { get; }
        void Follow(Entity target);
        void ClearFollow();
        void SetConfiner(Collider2D bounds);
    }

    public sealed class CinemachineCameraService : MonoBehaviour, ICameraService
    {
        private EntityViewService _entityViewService;

        [SerializeField] private CinemachineCamera _camera;

        [Inject]
        private void Construct(Camera camera, EntityViewService entityViewService)
        {
            Camera = camera;
            _entityViewService = entityViewService;
        }

        public Camera Camera { get; private set; }

        public void Follow(Entity target)
        {
            _camera.Target.TrackingTarget = _entityViewService.GetEntityView(target).transform;
        }

        public void ClearFollow()
        {
            _camera.Target.TrackingTarget = null;
        }

        public void SetConfiner(Collider2D bounds)
        {
            if (_camera.TryGetComponent<CinemachineConfiner2D>(out var confiner))
                confiner.BoundingShape2D = bounds;
        }
    }
}
