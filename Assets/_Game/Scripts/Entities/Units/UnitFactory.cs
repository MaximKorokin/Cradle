using Assets._Game.Scripts.Infrastructure.Game;
using Assets.CoreScripts;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Units
{
    public sealed class UnitFactory
    {
        private readonly PoolService _poolService;
        private readonly UnitVariantsCatalog _entityUnitVariantsCatalog;
        private readonly DefaultPrefabReferences _defaultPrefabReferences;

        public UnitFactory(PoolService poolService, UnitVariantsCatalog entityUnitVariantsCatalog, DefaultPrefabReferences defaultPrefabReferences)
        {
            _poolService = poolService;
            _entityUnitVariantsCatalog = entityUnitVariantsCatalog;
            _defaultPrefabReferences = defaultPrefabReferences;
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

            var unitVariant = unitVariants.GetVariant(variantName);
            if (unitVariant == null)
            {
                SLog.Error($"Cannot find unit variant by name: {variantName}");
                return null;
            }

            var unitView = CreateUnit(entityUnitVisualModel.Path.ToString());
            unitView.Path = entityUnitVisualModel.Path.ToString();
            unitView.RelativeOrderInLayer = entityUnitVisualModel.RelativeOrderInLayer;

            unitView.SpriteRenderer.sprite = unitVariant.Sprite;

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
