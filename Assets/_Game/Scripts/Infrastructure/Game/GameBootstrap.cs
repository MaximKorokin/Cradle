using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Units;
using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets._Game.Scripts.Items;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace Assets._Game.Scripts.Infrastructure.Game
{
    public class GameBootstrap : IStartable, ITickable
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

            quadruped = _entityAssembler.Create(_entityDefinitionCatalog.GetEntityDefinition("851ea68f-b985-4565-bbc0-816f9eb5ee8b"));

            SceneManager.LoadSceneAsync("UIRoot", LoadSceneMode.Additive);
        }

        Entity humanoid;
        Entity quadruped;
        public void Tick()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && humanoid.TryGetModule<UnitsController>(out var unitsController))
            {
                unitsController.UpdateOrderInLayer();

                _playerContext.IEModule.Inventory.Add(_itemStackAssembler.Create("e734e88a-6451-49f7-9777-bc1f36caa52d", 11).Snapshot);
                _playerContext.StashInventory.Add(_itemStackAssembler.Create("bc3b6314-c1ad-40d3-bbd7-2b5d8fdc2338", 1).Snapshot);
                _playerContext.StashInventory.Add(_itemStackAssembler.Create("e734e88a-6451-49f7-9777-bc1f36caa52f", 1).Snapshot);
                _playerContext.StashInventory.Add(_itemStackAssembler.Create("780db064-ca6a-4b9d-bc23-64e34a86403a", 1).Snapshot);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _saveService.SaveGame();
            }
        }
    }
}
