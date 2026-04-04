using Assets._Game.Scripts.Infrastructure.Game;
using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

namespace Assets._Game.Scripts.Entities
{
    public sealed class EntityViewLifecycleOrchestrator : IStartable, IDisposable
    {
        private readonly Dictionary<Entity, EntityView> _entitiesMapping = new();

        private readonly IGlobalEventBus _globalEventBus;
        private readonly EntityViewProvider _entityViewProvider;

        public EntityViewLifecycleOrchestrator(IGlobalEventBus bus, EntityViewProvider entityViewProvider)
        {
            _globalEventBus = bus;
            _entityViewProvider = entityViewProvider;

            _globalEventBus.Subscribe<SpawnEntityViewRequest>(OnSpawn);
            _globalEventBus.Subscribe<DespawnEntityViewRequest>(OnDespawn);
        }

        public void Dispose()
        {
            _globalEventBus.Unsubscribe<SpawnEntityViewRequest>(OnSpawn);
            _globalEventBus.Unsubscribe<DespawnEntityViewRequest>(OnDespawn);
        }

        // This is required to ensure that the orchestrator is created at the start of the game and subscribes to the events before any other system publishes them
        public void Start() { }

        private void OnSpawn(SpawnEntityViewRequest e)
        {
            var view = _entityViewProvider.Create(e.Entity.Definition);
            view.transform.position = e.Position;

            _entitiesMapping[e.Entity] = view;
            view.Bind(e.Entity);
        }

        private void OnDespawn(DespawnEntityViewRequest e)
        {
            if (!_entitiesMapping.TryGetValue(e.Entity, out var view))
            {
                SLog.Error($"No View found for Entity {e.Entity.Definition}");
                return;
            }
            _entityViewProvider.Destroy(view);
            _entitiesMapping.Remove(e.Entity);
        }
    }

    public readonly struct SpawnEntityViewRequest : IGlobalEvent
    {
        public readonly Entity Entity;
        public readonly Vector2 Position;

        public SpawnEntityViewRequest(Entity entity, Vector2 position)
        {
            Entity = entity;
            Position = position;
        }
    }

    public readonly struct DespawnEntityViewRequest : IGlobalEvent
    {
        public readonly Entity Entity;

        public DespawnEntityViewRequest(Entity entity)
        {
            Entity = entity;
        }
    }

    public readonly struct EntityDiedEvent : IGlobalEvent
    {
        public readonly Entity Victim;
        public readonly Entity Killer;

        public EntityDiedEvent(Entity victim, Entity killer)
        {
            Victim = victim;
            Killer = killer;
        }
    }
}
