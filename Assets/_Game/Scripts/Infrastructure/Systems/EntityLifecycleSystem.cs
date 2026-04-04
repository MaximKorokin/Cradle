using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Querying;
using Assets._Game.Scripts.Infrastructure.Storage;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class EntityLifecycleSystem : EntitySystemBase, ITickSystem
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly EntityFactory _entityFactory;

        protected override EntityQuery EntityQuery { get; } = new(RestrictionState.Disabled, new[] { typeof(DespawnModule) });

        public EntityLifecycleSystem(EntityRepository repository, IGlobalEventBus globalEventBus, EntityFactory entityFactory) : base(repository)
        {
            _globalEventBus = globalEventBus;
            _entityFactory = entityFactory;

            _globalEventBus.Subscribe<EntityDiedEvent>(OnEntityDied);
            _globalEventBus.Subscribe<SpawnEntityRequest>(OnEntitySpawnRequested);
            _globalEventBus.Subscribe<DespawnEntityRequest>(OnEntityDespawnRequested);
        }

        public override void Dispose()
        {
            base.Dispose();

            _globalEventBus.Unsubscribe<EntityDiedEvent>(OnEntityDied);
            _globalEventBus.Unsubscribe<SpawnEntityRequest>(OnEntitySpawnRequested);
            _globalEventBus.Unsubscribe<DespawnEntityRequest>(OnEntityDespawnRequested);
        }

        public void Tick(float delta)
        {
            IterateMatchingEntities(TickEntity);
        }

        private void TickEntity(Entity entity)
        {
            if (entity.GetModule<DespawnModule>().IsExpired)
            {
                _globalEventBus.Publish(new DespawnEntityRequest(entity));
            }
        }

        protected override void OnEntityAdded(Entity entity)
        {
            base.OnEntityAdded(entity);

            if (entity.TryGetModule<DespawnModule>(out var module))
            {
                if (module.Trigger == DespawnCounterStartTrigger.OnSpawn) module.StartDespawnTime = Time.time;
                else if (module.Trigger == DespawnCounterStartTrigger.OnDeath) module.StartDespawnTime = null;
            }
        }

        private void OnEntityDied(EntityDiedEvent e)
        {
            if (e.Victim.TryGetModule<DespawnModule>(out var module))
            {
                if (module.Trigger == DespawnCounterStartTrigger.OnDeath)
                {
                    module.StartDespawnTime = Time.time;
                }
            }
        }

        private void OnEntitySpawnRequested(SpawnEntityRequest request)
        {
            var entity = _entityFactory.Create(request.EntityDefinition);
            _globalEventBus.Publish<SpawnEntityViewRequest>(new(entity, request.Position));

            if (request.ModulesToAdd != null)
            {
                for (int i = 0; i < request.ModulesToAdd.Length; i++)
                {
                    entity.AddModule(request.ModulesToAdd[i]);
                }
            }
        }

        private void OnEntityDespawnRequested(DespawnEntityRequest request)
        {
            _globalEventBus.Publish(new DespawnEntityViewRequest(request.Entity));
            // For now entity does not exist if it does not have view
            // There will be a big TODO in the future if this will change
            EntityRepository.Remove(((IEntry)request.Entity).Id);
        }
    }

    public readonly struct SpawnEntityRequest : IGlobalEvent
    {
        public readonly EntityDefinition EntityDefinition;
        public readonly Vector2 Position;
        public readonly EntityModuleBase[] ModulesToAdd;

        public SpawnEntityRequest(EntityDefinition entityDefinition, Vector2 position, EntityModuleBase[] modulesToAdd = null)
        {
            EntityDefinition = entityDefinition;
            Position = position;
            ModulesToAdd = modulesToAdd;
        }
    }

    public readonly struct DespawnEntityRequest : IGlobalEvent
    {
        public readonly Entity Entity;
        public DespawnEntityRequest(Entity entity)
        {
            Entity = entity;
        }
    }
}
