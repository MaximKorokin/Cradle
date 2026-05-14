using Assets._Game.Scripts.Infrastructure.Configs;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Shared.Extensions;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Units
{
    public sealed class UnitViewProvider
    {
        private readonly PoolService _poolService;
        private readonly DefaultPrefabReferences _defaultPrefabReferences;
        private readonly EntityUnitConfig _entityUnitConfig;

        public UnitViewProvider(
            PoolService poolService,
            DefaultPrefabReferences defaultPrefabReferences,
            EntityUnitConfig entityUnitConfig)
        {
            _poolService = poolService;
            _defaultPrefabReferences = defaultPrefabReferences;
            _entityUnitConfig = entityUnitConfig;
        }

        public UnitView Create(string path, int relativeOrderInLayer)
        {
            var unitView = CreateUnit(path);
            unitView.Path = path;
            unitView.RelativeOrderInLayer = relativeOrderInLayer;
            return unitView;
        }

        public UnitView Create(EntityUnitVisualModel entityUnitVisualModel, Color color)
        {
            var unitView = CreateUnit(entityUnitVisualModel.Path.ToString());
            unitView.Path = entityUnitVisualModel.Path.ToString();
            unitView.RelativeOrderInLayer = entityUnitVisualModel.RelativeOrderInLayer;

            var unitVariantSprite = entityUnitVisualModel.SpriteVariants.GetRandomElement(_entityUnitConfig.NotFoundVariantSprite);
            unitView.SpriteRenderer.sprite = unitVariantSprite;
            unitView.SpriteRenderer.color = color;

            return unitView;
        }

        private UnitView CreateUnit(string name)
        {
            var unitView = _poolService.Take(_defaultPrefabReferences.UnitView, Vector3.zero, Quaternion.identity);
            unitView.gameObject.name = name;
            return unitView;
        }

        public void Destroy(UnitView unitView)
        {
            _poolService.Return(unitView);
        }
    }
}
