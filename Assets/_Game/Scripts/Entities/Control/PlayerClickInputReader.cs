using Assets._Game.Scripts.Infrastructure.Game;
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

        [Inject]
        public void Construct(ICameraService cameraService, PlayerControlProvider playerControlProvider)
        {
            _cameraService = cameraService;
            _playerControlProvider = playerControlProvider;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var clickPosition = _cameraService.Camera.ScreenToWorldPoint(eventData.position);
            _playerControlProvider.SetMoveTarget((Vector2)clickPosition);
        }
    }
}
