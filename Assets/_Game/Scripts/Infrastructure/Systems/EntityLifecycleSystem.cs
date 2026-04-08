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
        private readonly IGlobalEventBus _globalEventBus;
        private readonly EntityFactory _entityFactory;
        private readonly EntityViewService _entityViewService;

        protected override EntityQuery EntityQuery { get; } = new(RestrictionState.Disabled, new[] { typeof(DespawnModule) });

        public EntityLifecycleSystem(
            EntityRepository repository,
            IGlobalEventBus globalEventBus,
            EntityFactory entityFactory,
            EntityViewService entityViewService) : base(repository)
        {
            _globalEventBus = globalEventBus;
            _entityFactory = entityFactory;
            _entityViewService = entityViewService;

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
            EntityRepository.Add(entity);

            _entityViewService.SpawnEntityView(entity, request.Position);

            if (request.Initializers != null)
            {
                for (int i = 0; i < request.Initializers.Length; i++)
                {
                    request.Initializers[i].Initialize(entity);
                }
            }

            _globalEventBus.Publish(new EntitySpawnedEvent(entity));
        }

        private void OnEntityDespawnRequested(DespawnEntityRequest request)
        {
            _globalEventBus.Publish(new EntityDespawningEvent(request.Entity));

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

    public readonly struct PlayerSpawnedEvent : IGlobalEvent
    {
        public readonly Entity Player;
        public PlayerSpawnedEvent(Entity player)
        {
            Player = player;
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

    public class AppearanceEntitySpawnInitializer : IEntitySpawnInitializer
    {
        private readonly string _path;
        private readonly Sprite _sprite;

        public AppearanceEntitySpawnInitializer(Sprite sprite) : this(null, sprite) { }
        public AppearanceEntitySpawnInitializer(string path, Sprite sprite)
        {
            _path = path;
            _sprite = sprite;
        }

        public void Initialize(Entity entity)
        {
            if (!entity.TryGetModule<AppearanceModule>(out var appearanceModule)) return;

            if (string.IsNullOrWhiteSpace(_path))
                appearanceModule.RequestSetUnitSprite(_sprite);
            else 
                appearanceModule.RequestSetUnitSprite(_path, _sprite);
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
            if (_gameSave?.PlayerLocationSave == null)
            {
                _globalEventBus.Publish(new LocationTransitionRequest(_newGameDefinition.Location.Id, _newGameDefinition.LocationEntrance));
            }
            else
            {
                _globalEventBus.Publish(new LocationTransitionRequest(_gameSave.PlayerLocationSave.LocationId, null));

                var position = new Vector2(_gameSave.PlayerLocationSave.PositionX, _gameSave.PlayerLocationSave.PositionY);
                entity.Publish(new EntityRepositionRequest(position));
            }

            _globalEventBus.Publish(new PlayerSpawnedEvent(entity));
        }
    }
}
