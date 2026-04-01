using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
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
        private LocationDefinition _location;

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

            _globalEventBus.Publish<LocationTransitionRequest>(new(_location.Id, _targetEntranceId));
        }
    }
}
