using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Units;
using Assets._Game.Scripts.Infrastructure.Persistence;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace Assets._Game.Scripts.Infrastructure.Game
{
    public class GameBootstrap : IStartable, ITickable
    {
        private readonly EntityAssembler _entityBuilder;
        private readonly GameSaveRepository _repository;
        private readonly NewGameDefinition _newGameDefinition;
        private readonly EntityDefinitionCatalog _entityDefinitionCatalog;
        private readonly GameContext _gameContext;

        public GameBootstrap(
            EntityAssembler entityAssembler,
            GameSaveRepository repository,
            NewGameDefinition newGameDefinition,
            EntityDefinitionCatalog entityDefinitionCatalog,
            GameContext gameContext)
        {
            _entityBuilder = entityAssembler;
            _repository = repository;
            _newGameDefinition = newGameDefinition;
            _entityDefinitionCatalog = entityDefinitionCatalog;
            _gameContext = gameContext;
        }

        public void Start()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            Application.targetFrameRate = 60;

            _gameContext.SetStash(new(13));

            var gameSave = _repository.Load("Humanoid");
            if (gameSave != null)
            {
                humanoid = _entityBuilder.Assemble(_newGameDefinition.PlayerEntityDefinition, gameSave.PlayerSave);
            }
            else
            {
                humanoid = _entityBuilder.Create(_newGameDefinition.PlayerEntityDefinition);
            }
            _gameContext.SetPlayer(humanoid);
            if (humanoid.TryGetModule<EntityInventoryEquipmentModule>(out var inventoryEquipmentModule))
                _gameContext.SetIEModule(inventoryEquipmentModule);

            quadruped = _entityBuilder.Create(_entityDefinitionCatalog.GetEntityDefinition("851ea68f-b985-4565-bbc0-816f9eb5ee8b"));

            SceneManager.LoadSceneAsync("UIRoot", LoadSceneMode.Additive);
        }

        Entity humanoid;
        Entity quadruped;
        public void Tick()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && humanoid.TryGetModule<UnitsController>(out var unitsController))
            {
                unitsController.UpdateOrderInLayer();
            }
        }
    }
}