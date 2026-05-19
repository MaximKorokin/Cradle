using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets._Game.Scripts.Infrastructure.Querying;
using Assets._Game.Scripts.Infrastructure.Systems.Location;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class PersistenceSystem : EntitySystemBase
    {
        private readonly SaveService _saveService;
        private readonly NewGameDefinition _newGameDefinition;
        private readonly IPlayerProvider _playerProvider;

        protected override EntityQuery EntityQuery { get; } =
            new EntityQuery(
                requiredModules: new[] { typeof(PersistenceModule) }
            );

        public PersistenceSystem(
            IGlobalEventBus globalEventBus,
            EntityRepository repository,
            SaveService saveService,
            NewGameDefinition newGameDefinition,
            IPlayerProvider playerProvider) : base(globalEventBus, repository)
        {
            _saveService = saveService;
            _newGameDefinition = newGameDefinition;
            _playerProvider = playerProvider;

            TrackGlobalEvent<SaveGameRequest>(OnSaveGameRequested);
        }

        protected override void OnEntityAdded(Entity entity)
        {
            base.OnEntityAdded(entity);

            if (!EntityQuery.Match(entity)) return;

            _saveService.LoadEntity(entity);

            var entitySave = _saveService.GetEntitySave(entity);

            // Load player save data and transition to the correct location
            if (_playerProvider.Player == entity)
            {
                //ResetLevelSave();
                //ResetLocationSave();
                //ResetSave();

                LoadPlayerLocation(entity, entitySave);
            }
        }

        protected override void OnEntityRemoved(Entity entity)
        {
            base.OnEntityRemoved(entity);

            if (!EntityQuery.Match(entity)) return;

            _saveService.SaveEntity(entity);
        }

        private void LoadPlayerLocation(Entity entity, EntitySave playerSave)
        {
            if (playerSave?.LocationSave == null || string.IsNullOrWhiteSpace(playerSave?.LocationSave.LocationId))
            {
                GlobalEventBus.Publish(new LocationTransitionRequest(_newGameDefinition.LocationTransitionData.LocationDefinition.Id, _newGameDefinition.LocationTransitionData.EntranceDefinition.Id));
            }
            else
            {
                // null entrance id means that player will be placed in the location based on coordinates, not on specific entrance
                GlobalEventBus.Publish(new LocationTransitionRequest(playerSave?.LocationSave.LocationId, null));

                entity.SubscribeOnce<EntityViewBoundEvent>(_ =>
                {
                    var position = new Vector2(playerSave.LocationSave.PositionX, playerSave.LocationSave.PositionY);
                    entity.Publish(new EntityRepositionRequest(position));
                });
            }
        }

        private void OnSaveGameRequested(SaveGameRequest e)
        {
            IterateMatchingEntities(entity =>
            {
                _saveService.SaveEntity(entity);
            });
            _saveService.SaveGame();
        }
    }

    public readonly struct SaveGameRequest : IGlobalEvent { }
}
