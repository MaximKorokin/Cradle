using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Control;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets._Game.Scripts.Infrastructure.Querying;
using Assets._Game.Scripts.Items;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class EntityLifecycleSystem : EntitySystemBase, ITickSystem
    {
        private readonly EntityFactory _entityFactory;
        private readonly EntityViewService _entityViewService;

        protected override EntityQuery EntityQuery { get; } = new(RestrictionState.Disabled, new[] { typeof(DespawnModule) });

        public EntityLifecycleSystem(
            IGlobalEventBus globalEventBus,
            EntityRepository repository,
            EntityFactory entityFactory,
            EntityViewService entityViewService) : base(globalEventBus, repository)
        {
            _entityFactory = entityFactory;
            _entityViewService = entityViewService;

            TrackGlobalEvent<EntityDiedEvent>(OnEntityDied);
            TrackGlobalEvent<SpawnEntityRequest>(OnEntitySpawnRequested);
            TrackGlobalEvent<DespawnEntityRequest>(OnEntityDespawnRequested);
        }

        public void Tick(float delta)
        {
            IterateMatchingEntities(TickEntity);
        }

        private void TickEntity(Entity entity)
        {
            if (entity.GetModule<DespawnModule>().IsExpired)
            {
                GlobalEventBus.Publish(new DespawnEntityRequest(entity));
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
            // 1) Create entity with modules defined in the definition
            var entity = _entityFactory.Create(request.EntityDefinition);

            // 2) Apply any additional pre-add initialization logic (e.g. add more modules, set up module state, etc.)
            if (request.PreInitializers != null)
            {
                for (int i = 0; i < request.PreInitializers.Length; i++)
                {
                    request.PreInitializers[i].PreInitialize(entity);
                }
            }

            // 3) Initialize all modules (this will run the logic in the modules that depends on other modules being present
            foreach (var module in entity.Modules)
            {
                module.Initialize();
            }

            // 4) Add entity to repository so it can be found by other systems and modules
            EntityRepository.Add(entity);

            // 5) Apply post-add initialization logic (the entity is already observable through the repository)
            if (request.PostInitializers != null)
            {
                for (int i = 0; i < request.PostInitializers.Length; i++)
                {
                    request.PostInitializers[i].PostInitialize(entity);
                }
            }

            entity.MarkCreated();

            // 6) Spawn view for the entity
            _entityViewService.SpawnEntityView(entity, request.Position);

            GlobalEventBus.Publish(new EntitySpawnedEvent(entity));
        }

        private void OnEntityDespawnRequested(DespawnEntityRequest request)
        {
            GlobalEventBus.Publish(new EntityDespawningEvent(request.Entity));

            var stateModule = request.Entity.GetModule<RestrictionStateModule>();
            stateModule.Add(RestrictionState.Disabled);

            _entityViewService.DespawnEntityView(request.Entity);
            // For now entity does not exist if it does not have view
            // There will be a big TODO in the future if this will change
            EntityRepository.Remove(request.Entity.Id);
        }
    }

    public readonly struct SpawnEntityRequest : IGlobalEvent
    {
        public readonly EntityDefinition EntityDefinition;
        public readonly Vector2 Position;
        public readonly IEntityPreInitializer[] PreInitializers;
        public readonly IEntityPostInitializer[] PostInitializers;

        public SpawnEntityRequest(
            EntityDefinition entityDefinition,
            Vector2 position,
            IEntityPreInitializer[] preInitializers = null,
            IEntityPostInitializer[] postInitializers = null)
        {
            EntityDefinition = entityDefinition;
            Position = position;
            PreInitializers = preInitializers;
            PostInitializers = postInitializers;
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

    public readonly struct EntitySpawnedEvent : IGlobalEvent
    {
        public readonly Entity Entity;

        public EntitySpawnedEvent(Entity entity)
        {
            Entity = entity;
        }
    }

    public readonly struct EntityDespawningEvent : IGlobalEvent
    {
        public readonly Entity Entity;

        public EntityDespawningEvent(Entity entity)
        {
            Entity = entity;
        }
    }

    /// <summary>Runs before the entity is initialized and added to the repository.</summary>
    public interface IEntityPreInitializer
    {
        void PreInitialize(Entity entity);
    }

    /// <summary>Runs after the entity is added to the repository, before it is marked as created.</summary>
    public interface IEntityPostInitializer
    {
        void PostInitialize(Entity entity);
    }

    public class SpawnSourceEntityPreInitializer : IEntityPreInitializer
    {
        private readonly string _spawnSourceId;

        public SpawnSourceEntityPreInitializer(string spawnSourceId)
        {
            _spawnSourceId = spawnSourceId;
        }

        public void PreInitialize(Entity entity)
        {
            entity.AddModule(new SpawnSourceModule(_spawnSourceId));
        }
    }

    public class LootItemEntityPreInitializer : IEntityPreInitializer
    {
        private readonly ItemDefinition _itemDefinition;
        private readonly int _amount;

        public LootItemEntityPreInitializer(ItemDefinition itemDefinition, int amount)
        {
            _itemDefinition = itemDefinition;
            _amount = amount;
        }

        public void PreInitialize(Entity entity)
        {
            entity.AddModule(new LootItemModule(_itemDefinition, _amount));
        }
    }

    public class PlayerEntityPostInitializer : IEntityPostInitializer
    {
        private readonly PlayerContext _playerContext;

        public PlayerEntityPostInitializer(PlayerContext playerContext)
        {
            _playerContext = playerContext;
        }

        public void PostInitialize(Entity entity)
        {
            _playerContext.SetPlayer(entity);

            if (entity.TryGetModule<ControlModule>(out var controlModule))
            {
                foreach (var provider in controlModule.Providers)
                {
                    if (provider is AiControlProvider aiControlProvider)
                    {
                        aiControlProvider.SetEnabled(false);
                    }
                }
            }
        }
    }
}
