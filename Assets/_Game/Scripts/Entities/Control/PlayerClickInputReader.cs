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

        //private void Update()
        //{
        //    if (Pointer.current.press.wasPressedThisFrame)
        //    {
        //        var p = (Vector2)_camera.ScreenToWorldPoint(Pointer.current.position.ReadValue());
        //        _playerControlProvider.SetInput((p - (Vector2)transform.position).normalized);

        //        var hit = Physics2D.Raycast(p, Vector2.zero, 0f, _entityLayer);
        //        if (hit.collider != null && hit.collider.TryGetComponent<EntityView>(out var view))
        //        {
        //        }
        //        else
        //        {
        //        }
        //    }
        //}
    }
}
