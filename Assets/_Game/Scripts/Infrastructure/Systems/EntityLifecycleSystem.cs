using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets._Game.Scripts.Infrastructure.Querying;
using Assets._Game.Scripts.Infrastructure.Storage;
using Assets._Game.Scripts.Infrastructure.Systems.Location;
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

            // 2) Apply any additional initialization logic (e.g. add more modules, set up module state, etc.)
            if (request.Initializers != null)
            {
                for (int i = 0; i < request.Initializers.Length; i++)
                {
                    request.Initializers[i].Initialize(entity);
                }
            }

            // 3) Initialize all modules (this will run the logic in the modules that depends on other modules being present
            foreach (var module in entity.Modules)
            {
                module.Initialize();
            }

            // 4) Add entity to repository so it can be found by other systems and modules
            EntityRepository.Add(entity);

            // 5) Spawn view for the entity
            _entityViewService.SpawnEntityView(entity, request.Position);

            GlobalEventBus.Publish(new EntitySpawnedEvent(entity));
        }

        private void OnEntityDespawnRequested(DespawnEntityRequest request)
        {
            GlobalEventBus.Publish(new EntityDespawningEvent(request.Entity));

            _entityViewService.DespawnEntityView(request.Entity);
            // For now entity does not exist if it does not have view
            // There will be a big TODO in the future if this will change
            EntityRepository.Remove(((IEntry)request.Entity).Id);
        }
    }

    public readonly struct SpawnEntityRequest : IGlobalEvent
    {
        public readonly EntityDefinition EntityDefinition;
        public readonly Vector2 Position;
        public readonly IEntitySpawnInitializer[] Initializers;

        public SpawnEntityRequest(EntityDefinition entityDefinition, Vector2 position, IEntitySpawnInitializer[] initializers = null)
        {
            EntityDefinition = entityDefinition;
            Position = position;
            Initializers = initializers;
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

    public interface IEntitySpawnInitializer
    {
        void Initialize(Entity entity);
    }

    public class SpawnSourceEntitySpawnInitializer : IEntitySpawnInitializer
    {
        private readonly string _spawnSourceId;

        public SpawnSourceEntitySpawnInitializer(string spawnSourceId)
        {
            _spawnSourceId = spawnSourceId;
        }

        public void Initialize(Entity entity)
        {
            entity.AddModule(new SpawnSourceModule(_spawnSourceId));
        }
    }

    public class LootItemEntitySpawnInitializer : IEntitySpawnInitializer
    {
        private readonly ItemDefinition _itemDefinition;
        private readonly int _amount;

        public LootItemEntitySpawnInitializer(ItemDefinition itemDefinition, int amount)
        {
            _itemDefinition = itemDefinition;
            _amount = amount;
        }

        public void Initialize(Entity entity)
        {
            entity.AddModule(new LootItemModule(_itemDefinition, _amount));
        }
    }

    public class PlayerEntitySpawnInitializer : IEntitySpawnInitializer
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly NewGameDefinition _newGameDefinition;
        private readonly PlayerContext _playerContext;
        private readonly EntityFactory _entityFactory;
        private readonly GameSave _gameSave;

        public PlayerEntitySpawnInitializer(
            IGlobalEventBus globalEventBus,
            NewGameDefinition newGameDefinition,
            PlayerContext playerContext,
            EntityFactory entityFactory,
            GameSave gameSave)
        {
            _globalEventBus = globalEventBus;
            _newGameDefinition = newGameDefinition;
            _playerContext = playerContext;
            _entityFactory = entityFactory;
            _gameSave = gameSave;
        }

        public void Initialize(Entity entity)
        {
            // Apply player save data if it exists, otherwise the player will be in a default state.
            if (_gameSave?.PlayerSave != null)
            {
                _entityFactory.Apply(entity, _gameSave.PlayerSave);
            }
            _playerContext.SetPlayer(entity);

            // Load the location
            if (_gameSave?.PlayerLocationSave == null || string.IsNullOrWhiteSpace(_gameSave?.PlayerLocationSave.LocationId))
            {
                _globalEventBus.Publish(new LocationTransitionRequest(_newGameDefinition.Location.Id, _newGameDefinition.LocationEntrance.Id));
            }
            else
            {
                _globalEventBus.Publish(new LocationTransitionRequest(_gameSave.PlayerLocationSave.LocationId, null));

                var position = new Vector2(_gameSave.PlayerLocationSave.PositionX, _gameSave.PlayerLocationSave.PositionY);
                entity.Publish(new EntityRepositionRequest(position));
            }
        }
    }
}
