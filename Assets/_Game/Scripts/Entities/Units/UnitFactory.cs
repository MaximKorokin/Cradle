using Assets._Game.Scripts.Infrastructure.Configs;
using Assets._Game.Scripts.Infrastructure.Game;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Units
{
    public sealed class UnitFactory
    {
        private readonly PoolService _poolService;
        private readonly UnitVariantsCatalog _entityUnitVariantsCatalog;
        private readonly DefaultPrefabReferences _defaultPrefabReferences;
        private readonly EntityUnitConfig _entityUnitConfig;

        public UnitFactory(
            PoolService poolService,
            UnitVariantsCatalog entityUnitVariantsCatalog,
            DefaultPrefabReferences defaultPrefabReferences,
            EntityUnitConfig entityUnitConfig)
        {
            _poolService = poolService;
            _entityUnitVariantsCatalog = entityUnitVariantsCatalog;
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

        public UnitView Create(EntityUnitVisualModel entityUnitVisualModel, string variantName)
        {
            var unitVariants = _entityUnitVariantsCatalog.GetByPath(entityUnitVisualModel.Path);
            if (unitVariants == null)
            {
                SLog.Error($"Cannot find unit variants by path: {entityUnitVisualModel.Path}");
                return null;
            }

            var unitVariantSprite = unitVariants.GetVariant(variantName)?.Sprite;
            if (unitVariantSprite == null)
            {
                unitVariantSprite = _entityUnitConfig.NotFoundVariantSprite;
            }

            var unitView = CreateUnit(entityUnitVisualModel.Path.ToString());
            unitView.Path = entityUnitVisualModel.Path.ToString();
            unitView.RelativeOrderInLayer = entityUnitVisualModel.RelativeOrderInLayer;

            unitView.SpriteRenderer.sprite = unitVariantSprite;

            return unitView;
        }

        private UnitView CreateUnit(string name)
        {
            var unitView = _poolService.Take(_defaultPrefabReferences.UnitView, Vector3.zero, Quaternion.identity);
            unitView.gameObject.name = name;
            return unitView;
        }
    }
}
