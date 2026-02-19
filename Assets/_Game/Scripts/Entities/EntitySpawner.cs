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

            _bus.Subscribe<SpawnEntityEvent>(OnSpawn);
        }

        public void Start() { }

        private void OnSpawn(SpawnEntityEvent e)
        {
            if (!e.Entity.TryGetModule<AppearanceModule>(out var module))
            {
                SLog.Error($"Entity has no {nameof(AppearanceModule)} attached.");
                return;
            }

            var view = _entityViewFactory.Create(e.Entity.Definition);
            view.transform.position = e.Position;
            view.Bind(module);
        }
    }

    public readonly struct SpawnEntityEvent : IGlobalEvent
    {
        public readonly Entity Entity;
        public readonly Vector2 Position;

        public SpawnEntityEvent(Entity entity, Vector2 position)
        {
            Entity = entity;
            Position = position;
        }
    }
}
