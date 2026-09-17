using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Control;
using Assets._Game.Scripts.Infrastructure.Configs;
using Assets._Game.Scripts.Infrastructure.Game;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class PlayerControlSystem : SystemBase
    {
        private readonly ICameraService _cameraService;
        private readonly PlayerControlProvider _playerControlProvider;
        private readonly MoveTargetIndicatorView _moveTragetIndicatorView;

        private bool _isMoving = false;

        public PlayerControlSystem(
            ICameraService cameraService,
            PlayerControlProvider playerControlProvider,
            MoveTargetIndicatorConfig moveTargetIndicatorConfig,
            IGlobalEventBus globalEventBus) : base(globalEventBus)
        {
            _cameraService = cameraService;
            _playerControlProvider = playerControlProvider;

            _moveTragetIndicatorView = Object.Instantiate(moveTargetIndicatorConfig.Prefab);
            _moveTragetIndicatorView.gameObject.SetActive(false);

            TrackGlobalEvent<PointerDownEvent>(OnPointerDown);
            TrackGlobalEvent<PointerUpEvent>(OnPointerUp);
            TrackGlobalEvent<PointerMoveEvent>(OnPointerMove);
        }

        private void OnPointerDown(PointerDownEvent e)
        {
            if (e.Context.IsOverUI) return;

            _isMoving = true;
            var worldPosition = (Vector2)_cameraService.Camera.ScreenToWorldPoint(e.Context.ScreenPosition);
            _moveTragetIndicatorView.PlayAt(worldPosition);
        }

        private void OnPointerUp(PointerUpEvent e)
        {
            _isMoving = false;
        }

        private void OnPointerMove(PointerMoveEvent e)
        {
            if (e.Context.IsOverUI || !e.Context.IsPressed || !_isMoving) return;

            var worldPosition = (Vector2)_cameraService.Camera.ScreenToWorldPoint(e.Context.ScreenPosition);
            _playerControlProvider.SetMoveTarget(worldPosition);
        }
    }
}
