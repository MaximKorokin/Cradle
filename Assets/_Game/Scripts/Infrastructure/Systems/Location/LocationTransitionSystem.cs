using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Locations;
using Assets._Game.Scripts.Shared.Utils;
using Assets._Game.Scripts.UI.Systems;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Assets._Game.Scripts.Infrastructure.Systems.Location
{
    public sealed class LocationTransitionSystem : SystemBase
    {
        private readonly LocationManager _locationManager;
        private readonly EntityRepository _entityRepository;

        public LocationTransitionSystem(
            IGlobalEventBus globalEventBus,
            LocationManager locationManager,
            EntityRepository entityRepository) : base(globalEventBus)
        {
            _locationManager = locationManager;
            _entityRepository = entityRepository;

            TrackGlobalEvent<LocationTransitionPromptShowRequest>(OnLocationTransitionPromptShowRequested);
            TrackGlobalEvent<LocationTransitionPromptHideRequest>(OnLocationTransitionPromptHideRequested);
            TrackGlobalEvent<LocationTransitionRequest>(OnLocationTransitionRequested);
        }

        private void OnLocationTransitionPromptShowRequested(LocationTransitionPromptShowRequest request)
        {
            var entity = _entityRepository.Get(request.EntityId);
            if (MeetsRequirements(entity, request.LocationTransitionData))
            {
                GlobalEventBus.Publish(PlayerPromptRequest.GetShowRequest(
                    GetPromptText(request.LocationTransitionData, true),
                    "Enter",
                    () => GlobalEventBus.Publish(new LocationTransitionRequest(request.EntityId, request.LocationTransitionData))));
            }
            else
            {
                GlobalEventBus.Publish(PlayerPromptRequest.GetShowWithoutButtonRequest(
                    GetPromptText(request.LocationTransitionData, false)));
            }
        }

        private void OnLocationTransitionPromptHideRequested(LocationTransitionPromptHideRequest request)
        {
            GlobalEventBus.Publish(PlayerPromptRequest.GetHideRequest());
        }

        private void OnLocationTransitionRequested(LocationTransitionRequest request)
        {
            _ = HandleTransitionAsync(request);
        }

        private async Task HandleTransitionAsync(LocationTransitionRequest request)
        {
            var entity = _entityRepository.Get(request.EntityId);
            var locationId = request.LocationTransitionData.LocationDefinition.Id;
            var entranceId = request.LocationTransitionData.EntranceDefinition != null ? request.LocationTransitionData.EntranceDefinition.Id : null;
            var requirementsData = request.LocationTransitionData.RequirementsData;

            if (!MeetsRequirements(entity, request.LocationTransitionData))
            {
                SLog.Warn($"Entity '{request.EntityId}' does not meet the requirements to transition to location '{locationId}'.");
                return;
            }

            if (entity.TryGetModule<InventoryModule>(out _) && requirementsData.Items != null)
            {
                for (int i = 0; i < requirementsData.Items.Length; i++)
                {
                    var requiredItem = requirementsData.Items[i].Definition.Id;
                    var requiredAmount = requirementsData.Items[i].Amount;
                    GlobalEventBus.Publish(new ItemCommandRequest(new DestroyItemCommand(ItemContainerPath.Inventory(entity.Id), requiredItem, requiredAmount)));
                }
            }

            try
            {
                if (_locationManager.CurrentLocation == null)
                    await _locationManager.LoadInitialLocation(locationId, entranceId);
                else
                    await _locationManager.TransitToLocation(locationId, entranceId);
            }
            catch (Exception e)
            {
                SLog.Error($"Failed to transition to location '{locationId}' via entrance '{entranceId}': {e}");
            }
        }

        private bool MeetsRequirements(Entity entity, LocationTransitionData transitionData)
        {
            if (entity.TryGetModule<LevelingModule>(out var levelingModule) && levelingModule.Level < transitionData.RequirementsData.Level)
            {
                return false;
            }
            if (entity.TryGetModule<InventoryModule>(out var inventoryModule) && transitionData.RequirementsData.Items != null)
            {
                for (int i = 0; i < transitionData.RequirementsData.Items.Length; i++)
                {
                    var requiredItem = new ItemKey(transitionData.RequirementsData.Items[i].Definition.Id);
                    var requiredAmount = transitionData.RequirementsData.Items[i].Amount;
                    if (!inventoryModule.Inventory.Has(requiredItem, requiredAmount))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private string GetPromptText(LocationTransitionData transitionData, bool canEnter)
        {
            var statusText = canEnter ? "Entrance to" : "Cannot enter";
            var requiredItemsText = transitionData.RequirementsData.Items != null
                ? "Consumes: " + string.Join(", ", transitionData.RequirementsData.Items.Select(i => $"{i.Definition.Name} ({i.Amount})"))
                : string.Empty;
            return $"<size=80%><color=#888888>{statusText}</color></size>{Environment.NewLine}" +
                   $"{transitionData.LocationDefinition.DisplayName} " +
                   $"<size=60%><color=#888888>(Lv. {transitionData.RequirementsData.Level}+)" +
                   $"{(string.IsNullOrWhiteSpace(requiredItemsText) ? string.Empty : Environment.NewLine) + requiredItemsText}</color></size>";
        }
    }

    public readonly struct LocationTransitionPromptShowRequest : IGlobalEvent
    {
        public readonly string EntityId;
        public readonly LocationTransitionData LocationTransitionData;

        public LocationTransitionPromptShowRequest(string entityId, LocationTransitionData locationTransitionData)
        {
            EntityId = entityId;
            LocationTransitionData = locationTransitionData;
        }
    }

    public readonly struct LocationTransitionPromptHideRequest : IGlobalEvent
    {
        public readonly string EntityId;
        public readonly LocationTransitionData LocationTransitionData;

        public LocationTransitionPromptHideRequest(string entityId, LocationTransitionData locationTransitionData)
        {
            EntityId = entityId;
            LocationTransitionData = locationTransitionData;
        }
    }

    public readonly struct LocationTransitionRequest : IGlobalEvent
    {
        public readonly string EntityId;
        public readonly LocationTransitionData LocationTransitionData;

        public LocationTransitionRequest(string entityId, LocationTransitionData locationTransitionData)
        {
            EntityId = entityId;
            LocationTransitionData = locationTransitionData;
        }
    }
}
