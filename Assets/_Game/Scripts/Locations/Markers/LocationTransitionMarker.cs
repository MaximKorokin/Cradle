using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems.Location;
using Assets._Game.Scripts.Shared.Unity;
using Assets._Game.Scripts.UI.Systems;
using System;
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
        private LocationTransitionData _locationTransitionData;

        [Inject]
        private void Construct(IGlobalEventBus globalEventBus, IPlayerProvider playerProvider)
        {
            _globalEventBus = globalEventBus;
            _playerProvider = playerProvider;
        }

        private void Awake()
        {
            _transitionTrigger.OnTriggerEntered += OnTriggerEntered;
            _transitionTrigger.OnTriggerExited += OnTriggerExited;
        }

        private void OnTriggerEntered(Collider2D collider)
        {
            if (!collider.TryGetComponent<EntityView>(out var entityView) || entityView.Entity != _playerProvider.Player)
                return;

            _globalEventBus.Publish(InteractionPromptViewRequest.ShowRequest(
                "<size=80%><color=#888888>Entrance to</color></size>" + Environment.NewLine + _locationTransitionData.LocationDefinition.DisplayName, "Enter", OnEnterPressed));
        }

        private void OnTriggerExited(Collider2D collider)
        {
            if (!collider.TryGetComponent<EntityView>(out var entityView) || entityView.Entity != _playerProvider.Player)
                return;

            _globalEventBus.Publish(InteractionPromptViewRequest.HideRequest());
        }

        private void OnEnterPressed()
        {
            _globalEventBus.Publish(new LocationTransitionRequest(_locationTransitionData.LocationDefinition.Id, _locationTransitionData.EntranceDefinition.Id));
        }
    }
}
