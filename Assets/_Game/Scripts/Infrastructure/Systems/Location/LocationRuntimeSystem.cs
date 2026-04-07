using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Locations;
using Assets._Game.Scripts.Locations.Markers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Systems.Location
{
    public sealed class LocationRuntimeSystem : SystemBase, ILocationSystem, IStartSystem, ITickSystem
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly EntityRepository _entityRepository;

        private readonly IReadOnlyList<EntitySpawnSpotRuntime> _entitySpawnSpots;

        public LocationRuntimeSystem(IGlobalEventBus globalEventBus, IReadOnlyList<EntitySpawnSpotRuntime> entitySpawnSpots, EntityRepository entityRepository)
        {
            _globalEventBus = globalEventBus;
            _entitySpawnSpots = entitySpawnSpots;
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
            IterateSpots(_entitySpawnSpots, _globalEventBus);
        }

        public void Tick(float delta)
        {
            IterateSpots(_entitySpawnSpots, _globalEventBus);
        }

        private void OnEntityDied(EntityDiedEvent e)
        {
            if (!e.Victim.TryGetModule<SpawnSourceModule>(out var spawnSourceModule)) return;

            for (int i = 0; i < _entitySpawnSpots.Count; i++)
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
                // Disable and despawn all entities that are spawned on the current location
                if (entity.TryGetModule<SpawnSourceModule>(out var spawnSourceModule))
                {
                    if (_entitySpawnSpots.Any(s => s.Id == spawnSourceModule.SourceId))
                    {
                        DisableAndDespawnEntity(entity);
                    }
                }
                // Disable and despawn all loot
                if (entity.TryGetModule<LootItemModule>(out var lootItemModule))
                {
                    DisableAndDespawnEntity(entity);
                }
            }
        }

        private void DisableAndDespawnEntity(Entity entity)
        {
            if (entity.TryGetModule<RestrictionStateModule>(out var restrictionStateModule))
                restrictionStateModule.Add(RestrictionState.Disabled);

            _globalEventBus.Publish(new DespawnEntityRequest(entity));
        }

        private static void IterateSpots(IReadOnlyList<EntitySpawnSpotRuntime> spots, IGlobalEventBus globalEventBus)
        {
            for (int i = 0; i < spots.Count; i++)
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
