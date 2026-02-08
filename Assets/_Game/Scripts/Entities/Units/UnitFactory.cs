using Assets.CoreScripts;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Units
{
    public sealed class UnitFactory
    {
        private readonly UnitVariantsCatalog _entityUnitVariantsCatalog;

        public UnitFactory(UnitVariantsCatalog entityUnitVariantsCatalog)
        {
            _entityUnitVariantsCatalog = entityUnitVariantsCatalog;
        }

        public Unit Create(string path, int relativeOrderInLayer)
        {
            var entityUnitGameObject = CreateUnit(path);
            var entityUnit = new Unit(entityUnitGameObject, path, relativeOrderInLayer);
            return entityUnit;
        }

        public Unit Create(EntityUnitVisualModel entityUnitVisualModel, string variantName)
        {
            var unitVariants = _entityUnitVariantsCatalog.GetUnit(entityUnitVisualModel.Path);
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

            var entityUnitGameObject = CreateUnit(entityUnitVisualModel.Path.ToString());
            var entityUnit = new Unit(entityUnitGameObject, entityUnitVisualModel.Path.ToString(), entityUnitVisualModel.RelativeOrderInLayer);

            entityUnit.SetSprite(unitVariant.Sprite);

            return entityUnit;
        }

        private GameObject CreateUnit(string name)
        {
            var entityUnitGameObject = new GameObject(name);
            entityUnitGameObject.AddComponent<SpriteRenderer>();
            return entityUnitGameObject;
        }
    }
}
