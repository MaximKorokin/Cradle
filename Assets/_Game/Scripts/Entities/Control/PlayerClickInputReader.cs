using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Assets._Game.Scripts.Entities.Control
{
    public sealed class PlayerClickInputReader : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private LayerMask _entityLayer;

        private PlayerControlProvider _playerControlProvider;

        [Inject]
        public void Construct(PlayerControlProvider playerControlProvider)
        {
            _playerControlProvider = playerControlProvider;
            if (_camera == null) _camera = Camera.main;
        }

        private void Update()
        {
            if (Pointer.current.press.wasPressedThisFrame)
            {
                var p = (Vector2)_camera.ScreenToWorldPoint(Pointer.current.position.ReadValue());
                _playerControlProvider.SetInput((p - (Vector2)transform.position).normalized);

                //var hit = Physics2D.Raycast(p, Vector2.zero, 0f, _entityLayer);
                //if (hit.collider != null && hit.collider.TryGetComponent<EntityView>(out var view))
                //{
                //}
                //else
                //{
                //}
            }
        }
    }
}
