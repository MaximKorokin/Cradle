using Assets.CoreScripts;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Units
{
    public sealed class EntityUnitFactory
    {
        private readonly EntityUnitVariantsCatalog _entityUnitVariantsCatalog;

        public EntityUnitFactory(EntityUnitVariantsCatalog entityUnitVariantsCatalog)
        {
            _entityUnitVariantsCatalog = entityUnitVariantsCatalog;
        }

        public EntityUnit Create(string path, Sprite sprite, int relativeOrderInLayer)
        {
            var entityUnitGameObject = CreateUnit(path);
            var entityUnit = new EntityUnit(entityUnitGameObject, path, relativeOrderInLayer);
            entityUnit.Set(sprite);
            return entityUnit;
        }

        public EntityUnit Create(EntityUnitVisualModel entityUnitVisualModel, string variantName)
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
            var entityUnit = new EntityUnit(entityUnitGameObject, entityUnitVisualModel.Path.ToString(), entityUnitVisualModel.RelativeOrderInLayer);

            entityUnit.Set(unitVariant.Sprite);

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
