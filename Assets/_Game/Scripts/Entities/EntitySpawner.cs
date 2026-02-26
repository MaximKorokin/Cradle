using Assets._Game.Scripts.Entities.Modules;
using Assets.CoreScripts;
using UnityEngine;
using VContainer.Unity;

namespace Assets._Game.Scripts.Entities
{
    public sealed class EntitySpawner : IStartable
    {
        private readonly IGlobalEventBus _bus;
        private readonly EntityViewFactory _entityViewFactory;

        public EntitySpawner(IGlobalEventBus bus, EntityViewFactory entityViewFactory)
        {
            _bus = bus;
            _entityViewFactory = entityViewFactory;

            _bus.Subscribe<SpawnEntityRequestEvent>(OnSpawn);
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
}
