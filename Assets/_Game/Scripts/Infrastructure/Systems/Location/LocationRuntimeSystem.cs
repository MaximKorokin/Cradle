using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Locations;
using Assets._Game.Scripts.Locations.Core;
using Assets._Game.Scripts.Locations.Markers;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Systems.Location
{
    public sealed class LocationRuntimeSystem : SystemBase, ILocationSystem, IStartSystem, ITickSystem
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly LocationMarkersContext _locationMarkersContext;
        private readonly EntityRepository _entityRepository;

        private EntitySpawnSpotRuntime[] _entitySpawnSpots;

        public LocationRuntimeSystem(IGlobalEventBus globalEventBus, LocationMarkersContext locationMarkersContext, EntityRepository entityRepository)
        {
            _globalEventBus = globalEventBus;
            _locationMarkersContext = locationMarkersContext;
            _entityRepository = entityRepository;

            _globalEventBus.Subscribe<EntityDiedEvent>(OnEntityDied);
            _globalEventBus.Subscribe<LocationChangingEvent>(OnLocationChangingEvent);
        }

        public override void Dispose()
        {
            base.Dispose();

            _globalEventBus.Unsubscribe<EntityDiedEvent>(OnEntityDied);
            _globalEventBus.Unsubscribe<LocationChangingEvent>(OnLocationChangingEvent);
        }

        public void Start()
        {
            // todo: move runtimes creation to context
            _entitySpawnSpots = _locationMarkersContext.EntitySpawnSpotMarkers
                .Where(m => m.SpawnOnLocationLoad)
                .Select(m => m.Definition.CreateRuntime(m.transform.position))
                .ToArray();

            IterateSpots(_entitySpawnSpots, _globalEventBus);
        }

        public void Tick(float delta)
        {
            IterateSpots(_entitySpawnSpots, _globalEventBus);
        }

        private void OnEntityDied(EntityDiedEvent e)
        {
            if (!e.Victim.TryGetModule<SpawnSourceModule>(out var spawnSourceModule)) return;

            for (int i = 0; i < _entitySpawnSpots.Length; i++)
            {
                if (spawnSourceModule.SourceId != _entitySpawnSpots[i].Id) continue;

                var spot = _entitySpawnSpots[i];
                spot.MarkDespawned(e.Victim.Definition.Id);
                break;
            }
        }

        private void OnLocationChangingEvent(LocationChangingEvent e)
        {
            foreach (var entity in _entityRepository.All.ToArray())
            {
                // Despawn all entities that are spawned on the current location
                if (entity.TryGetModule<SpawnSourceModule>(out var spawnSourceModule))
                {
                    if (_entitySpawnSpots.Any(s => s.Id == spawnSourceModule.SourceId))
                    {
                        _globalEventBus.Publish(new DespawnEntityRequest(entity));
                    }
                }
                // Despawn all loot
                if (entity.TryGetModule<LootItemModule>(out var lootItemModule))
                {
                    _globalEventBus.Publish(new DespawnEntityRequest(entity));
                }
            }
        }

        private static void IterateSpots(EntitySpawnSpotRuntime[] spots, IGlobalEventBus globalEventBus)
        {
            for (int i = 0; i < spots.Length; i++)
            {
                var spot = spots[i];
                for (int j = 0; j < spot.EntityDefinitions.Count; j++)
                {
                    var entityDefinition = spot.EntityDefinitions[j];
                    var spawnAmount = spot.GetSpawnAmount(entityDefinition.Id);
                    for (int k = 0; k < spawnAmount; k++)
                    {
                        globalEventBus.Publish(new SpawnEntityRequest(
                            entityDefinition,
                            GetSpawnPosition(spot.Center, spot.Radius),
                            new[] { new SpawnSourceEntitySpawnInitializer(spot.Id) }));
                        spot.MarkSpawned(entityDefinition.Id);
                    }
                }
            }
        }

        private static Vector2 GetSpawnPosition(Vector2 center, float radius)
        {
            return center + Random.insideUnitCircle * radius;
        }
    }

    public readonly struct LocationLoadedEvent
    {
        public readonly string LocationId;
        public LocationLoadedEvent(string locationId)
        {
            LocationId = locationId;
        }
    }
}
