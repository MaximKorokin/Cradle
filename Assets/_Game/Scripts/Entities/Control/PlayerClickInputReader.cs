using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace Assets._Game.Scripts.Entities.Control
{
    public sealed class PlayerClickInputReader : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private LayerMask _entityLayer;

        private Camera _camera;
        private PlayerControlProvider _playerControlProvider;

        [Inject]
        public void Construct(PlayerControlProvider playerControlProvider)
        {
            _camera = Camera.main;

            _playerControlProvider = playerControlProvider;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var clickPosition = _camera.ScreenToWorldPoint(eventData.position);
            _playerControlProvider.SetMoveTarget((Vector2)clickPosition);
        }
    }
}
