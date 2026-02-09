using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets._Game.Scripts.Items;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace Assets._Game.Scripts.Infrastructure.Game
{
    public class GameBootstrap : IStartable
    {
        private readonly ItemStackAssembler _itemStackAssembler;
        private readonly EntityAssembler _entityAssembler;
        private readonly GameSaveRepository _gameSaveRepository;
        private readonly NewGameDefinition _newGameDefinition;
        private readonly EntityDefinitionCatalog _entityDefinitionCatalog;
        private readonly PlayerContext _playerContext;
        private readonly SaveService _saveService;

        public GameBootstrap(
            ItemStackAssembler itemStackAssembler,
            EntityAssembler entityAssembler,
            GameSaveRepository repository,
            NewGameDefinition newGameDefinition,
            EntityDefinitionCatalog entityDefinitionCatalog,
            PlayerContext playerContext,
            SaveService saveService)
        {
            _itemStackAssembler = itemStackAssembler;
            _entityAssembler = entityAssembler;
            _gameSaveRepository = repository;
            _newGameDefinition = newGameDefinition;
            _entityDefinitionCatalog = entityDefinitionCatalog;
            _playerContext = playerContext;
            _saveService = saveService;
        }

        public void Start()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            Application.targetFrameRate = 60;

            _playerContext.SetStash(new(13));

            Entity humanoid;
            var gameSave = _gameSaveRepository.Load("Player");
            if (gameSave != null)
            {
                humanoid = _entityAssembler.Assemble(_newGameDefinition.PlayerEntityDefinition, gameSave.PlayerSave);
            }
            else
            {
                humanoid = _entityAssembler.Create(_newGameDefinition.PlayerEntityDefinition);
            }
            _playerContext.SetPlayer(humanoid);

            var quadruped = _entityAssembler.Create(_entityDefinitionCatalog.GetEntityDefinition("851ea68f-b985-4565-bbc0-816f9eb5ee8b"));

            SceneManager.LoadSceneAsync("UIRoot", LoadSceneMode.Additive);
        }
    }
}
