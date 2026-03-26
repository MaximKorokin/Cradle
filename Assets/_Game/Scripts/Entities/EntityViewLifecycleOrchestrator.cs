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

            _globalEventBus.Subscribe<SpawnEntityViewRequestEvent>(OnSpawn);
            _globalEventBus.Subscribe<DespawnEntityViewRequestEvent>(OnDespawn);
        }

        public void Dispose()
        {
            _globalEventBus.Unsubscribe<SpawnEntityViewRequestEvent>(OnSpawn);
            _globalEventBus.Unsubscribe<DespawnEntityViewRequestEvent>(OnDespawn);
        }

        public void Start() { }

        private void OnSpawn(SpawnEntityViewRequestEvent e)
        {
            var view = _entityViewProvider.Create(e.Entity.Definition);
            view.transform.position = e.Position;

            _entitiesMapping[e.Entity] = view;
            view.Bind(e.Entity);
        }

        private void OnDespawn(DespawnEntityViewRequestEvent e)
        {
            if (!_entitiesMapping.TryGetValue(e.Entity, out var view))
            {
                SLog.Error($"No View found for Entity {e.Entity}");
                return;
            }
            _entityViewProvider.Destroy(view);
            _entitiesMapping.Remove(e.Entity);
        }
    }

    public readonly struct SpawnEntityViewRequestEvent : IGlobalEvent
    {
        public readonly Entity Entity;
        public readonly Vector2 Position;

        public SpawnEntityViewRequestEvent(Entity entity, Vector2 position)
        {
            Entity = entity;
            Position = position;
        }
    }

    public readonly struct DespawnEntityViewRequestEvent : IGlobalEvent
    {
        public readonly Entity Entity;

        public DespawnEntityViewRequestEvent(Entity entity)
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
