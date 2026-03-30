using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets._Game.Scripts.Locations;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace Assets._Game.Scripts.Infrastructure.Game
{
    public class GameBootstrap : IStartable
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly SaveService _saveService;
        private readonly EntityFactory _entityAssembler;
        private readonly EntityDefinitionCatalog _entityDefinitionCatalog;

        public GameBootstrap(
            IGlobalEventBus globalEventBus,
            SaveService saveService,
            EntityFactory entityAssembler,
            EntityDefinitionCatalog entityDefinitionCatalog)
        {
            _globalEventBus = globalEventBus;
            _saveService = saveService;
            _entityAssembler = entityAssembler;
            _entityDefinitionCatalog = entityDefinitionCatalog;
        }

        public void Start()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            Application.targetFrameRate = 60;

            _saveService.LoadGame();

            var dogDefinition = _entityDefinitionCatalog.Get("d5855ca383df4ddd8c615e51609dc812");
            var dogEntity1 = _entityAssembler.Create(dogDefinition);
            _globalEventBus.Publish(new SpawnEntityViewRequestEvent(dogEntity1, Vector2.one));
            var dogEntity2 = _entityAssembler.Create(dogDefinition);
            _globalEventBus.Publish(new SpawnEntityViewRequestEvent(dogEntity2, -Vector2.one));

            SceneManager.LoadSceneAsync("UIRoot", LoadSceneMode.Additive);
        }
    }
}
