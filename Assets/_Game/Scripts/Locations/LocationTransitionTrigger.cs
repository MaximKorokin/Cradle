using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Infrastructure.Game;
using UnityEngine;
using VContainer;

namespace Assets._Game.Scripts.Locations
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class LocationTransitionTrigger : MonoBehaviour
    {
        private IGlobalEventBus _globalEventBus;
        private IPlayerProvider _playerProvider;

        [SerializeField]
        private string _targetLocationId;

        [SerializeField]
        private string _targetEntranceId;

        [Inject]
        private void Construct(IGlobalEventBus globalEventBus, IPlayerProvider playerProvider)
        {
            _globalEventBus = globalEventBus;
            _playerProvider = playerProvider;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent<EntityView>(out var entityView) && entityView.Entity != _playerProvider.Player)
                return;

            _globalEventBus.Publish<RequestLocationTransitionEvent>(new(_targetLocationId, _targetEntranceId));
        }
    }

    public readonly struct RequestLocationTransitionEvent : IGlobalEvent
    {
        public readonly string TargetLocationId;
        public readonly string TargetEntranceId;

        public RequestLocationTransitionEvent(string targetLocationId, string targetEntranceId)
        {
            TargetLocationId = targetLocationId;
            TargetEntranceId = targetEntranceId;
        }
    }
}
