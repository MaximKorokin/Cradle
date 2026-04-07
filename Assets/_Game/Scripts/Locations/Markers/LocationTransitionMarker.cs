using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Infrastructure.Systems.Location;
using Assets._Game.Scripts.Shared.Unity;
using UnityEngine;
using VContainer;

namespace Assets._Game.Scripts.Locations.Markers
{
    public sealed class LocationTransitionMarker : MonoBehaviour
    {
        private IGlobalEventBus _globalEventBus;
        private IPlayerProvider _playerProvider;

        [SerializeField]
        private Trigger2D _transitionTrigger;

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

        private void Awake()
        {
            _transitionTrigger.OnTriggerEntered += OnTriggerEntered;
        }

        private void OnTriggerEntered(Collider2D collider)
        {
            if (!collider.TryGetComponent<EntityView>(out var entityView) || entityView.Entity != _playerProvider.Player)
                return;

            _globalEventBus.Publish(new LocationTransitionRequest(_location.Id, _targetEntranceId));
        }
    }
}
