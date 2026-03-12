using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Infrastructure.Persistence;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace Assets._Game.Scripts.Infrastructure.Game
{
    public class GameBootstrap : IStartable
    {
        private readonly IGlobalEventBus _eventBus;
        private readonly SaveService _saveService;
        private readonly EntityAssembler _entityAssembler;
        private readonly EntityDefinitionCatalog _entityDefinitionCatalog;

        public GameBootstrap(
            IGlobalEventBus eventBus,
            SaveService saveService,
            EntityAssembler entityAssembler,
            EntityDefinitionCatalog entityDefinitionCatalog)
        {
            _eventBus = eventBus;
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
            _eventBus.Publish(new SpawnEntityViewRequestEvent(dogEntity1, Vector2.one));
            var dogEntity2 = _entityAssembler.Create(dogDefinition);
            _eventBus.Publish(new SpawnEntityViewRequestEvent(dogEntity2, -Vector2.one));

            SceneManager.LoadSceneAsync("UIRoot", LoadSceneMode.Additive);
        }
    }
}
