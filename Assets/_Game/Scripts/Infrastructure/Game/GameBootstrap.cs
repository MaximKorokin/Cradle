using Assets._Game.Scripts.Entities;
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

        public GameBootstrap(
            EntityAssembler entityAssembler,
            GameSaveRepository repository,
            NewGameDefinition newGameDefinition,
            EntityDefinitionCatalog entityDefinitionCatalog)
        {
            _entityBuilder = entityAssembler;
            _repository = repository;
            _newGameDefinition = newGameDefinition;
            _entityDefinitionCatalog = entityDefinitionCatalog;
        }

        public void Start()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            Application.targetFrameRate = 60;

            var gameSave = _repository.Load("Humanoid");
            if (gameSave != null)
            {
                humanoid = _entityBuilder.Assemble(_newGameDefinition.PlayerEntityDefinition, gameSave.PlayerSave);
            }
            else
            {
                humanoid = _entityBuilder.Create(_newGameDefinition.PlayerEntityDefinition);
            }

            quadruped = _entityBuilder.Create(_entityDefinitionCatalog.GetEntityDefinition("851ea68f-b985-4565-bbc0-816f9eb5ee8b"));

            SceneManager.LoadSceneAsync("UIRoot", LoadSceneMode.Additive);
        }

        Entity humanoid;
        Entity quadruped;
        public void Tick()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                humanoid.UnitsController.UpdateOrderInLayer();
            }
        }
    }
}