using System;
using UnityEngine;
using VContainer.Unity;

namespace Assets._Game.Scripts.Entities
{
    public sealed class EntitySpawner : IStartable, IDisposable
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly EntityViewFactory _entityViewFactory;

        public EntitySpawner(IGlobalEventBus bus, EntityViewFactory entityViewFactory)
        {
            _globalEventBus = bus;
            _entityViewFactory = entityViewFactory;

            _globalEventBus.Subscribe<SpawnEntityRequestEvent>(OnSpawn);
        }

        public void Dispose()
        {
            _globalEventBus.Unsubscribe<SpawnEntityRequestEvent>(OnSpawn);
        }

        public void Start() { }

        private void OnSpawn(SpawnEntityRequestEvent e)
        {
            var view = _entityViewFactory.Create(e.Entity.Definition);
            view.transform.position = e.Position;
            view.Bind(e.Entity);
        }
    }

    public readonly struct SpawnEntityRequestEvent : IGlobalEvent
    {
        public readonly Entity Entity;
        public readonly Vector2 Position;

        public SpawnEntityRequestEvent(Entity entity, Vector2 position)
        {
            Entity = entity;
            Position = position;
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
