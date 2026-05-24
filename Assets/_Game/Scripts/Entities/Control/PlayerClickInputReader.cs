using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace Assets._Game.Scripts.Entities.Control
{
    public sealed class PlayerClickInputReader : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private LayerMask _entityLayer;

        private ICameraService _cameraService;
        private PlayerControlProvider _playerControlProvider;
        private IGlobalEventBus _globalEventBus;

        [Inject]
        public void Construct(ICameraService cameraService, PlayerControlProvider playerControlProvider, IGlobalEventBus globalEventBus)
        {
            _cameraService = cameraService;
            _playerControlProvider = playerControlProvider;
            _globalEventBus = globalEventBus;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var worldPosition = (Vector2)_cameraService.Camera.ScreenToWorldPoint(eventData.position);
            _playerControlProvider.SetMoveTarget(worldPosition);
            _globalEventBus.Publish(new PlayerMoveTargetIndicatorRequest(worldPosition));
        }
    }
}
