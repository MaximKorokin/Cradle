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
        private readonly SaveService _saveService;
        private readonly EntityAssembler _entityAssembler;
        private readonly EntityViewFactory _entityViewFactory;
        private readonly EntityDefinitionCatalog _entityDefinitionCatalog;

        public GameBootstrap(
            SaveService saveService,
            EntityAssembler entityAssembler,
            EntityViewFactory entityViewFactory,
            EntityDefinitionCatalog entityDefinitionCatalog)
        {
            _saveService = saveService;
            _entityAssembler = entityAssembler;
            _entityViewFactory = entityViewFactory;
            _entityDefinitionCatalog = entityDefinitionCatalog;
        }

        public void Start()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            Application.targetFrameRate = 60;

            _saveService.LoadGame();

            var quadrupedDefinition = _entityDefinitionCatalog.Get("d5855ca383df4ddd8c615e51609dc812");
            var quadruped = _entityAssembler.Create(quadrupedDefinition);
            var quadrupedView = _entityViewFactory.Create(quadrupedDefinition);
            //quadrupedView.Bind();

            SceneManager.LoadSceneAsync("UIRoot", LoadSceneMode.Additive);
        }
    }
}
