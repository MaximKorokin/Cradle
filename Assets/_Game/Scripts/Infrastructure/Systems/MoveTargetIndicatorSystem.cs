using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Infrastructure.Configs;
using Assets._Game.Scripts.Infrastructure.Game;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class MoveTargetIndicatorSystem : SystemBase
    {
        private readonly MoveTargetIndicatorView _view;

        public MoveTargetIndicatorSystem(
            IGlobalEventBus globalEventBus,
            MoveTargetIndicatorConfig config) : base(globalEventBus)
        {
            _view = Object.Instantiate(config.Prefab);
            _view.gameObject.SetActive(false);

            TrackGlobalEvent<PlayerMoveTargetIndicatorRequest>(OnMoveTargetIndicatorRequested);
        }

        private void OnMoveTargetIndicatorRequested(PlayerMoveTargetIndicatorRequest request)
        {
            _view.PlayAt(request.WorldPosition);
        }
    }

    public readonly struct PlayerMoveTargetIndicatorRequest : IGlobalEvent
    {
        public Vector2 WorldPosition { get; }

        public PlayerMoveTargetIndicatorRequest(Vector2 worldPosition)
        {
            WorldPosition = worldPosition;
        }
    }
}
