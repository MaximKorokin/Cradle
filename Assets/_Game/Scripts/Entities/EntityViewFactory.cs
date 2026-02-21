using Assets._Game.Scripts.Entities.Units;
using Assets._Game.Scripts.Infrastructure.Game;
using UnityEngine;

namespace Assets._Game.Scripts.Entities
{
    public sealed class EntityViewFactory
    {
        private static int _entitiesCounter = 0;

        private readonly PoolService _poolService;
        private readonly DefaultPrefabReferences _defaultPrefabReferences;
        private readonly UnitsControllerFactory _unitsControllerFactory;

        public EntityViewFactory(
            PoolService poolService,
            DefaultPrefabReferences defaultPrefabReferences,
            UnitsControllerFactory unitsControllerFactory)
        {
            _poolService = poolService;
            _defaultPrefabReferences = defaultPrefabReferences;
            _unitsControllerFactory = unitsControllerFactory;
        }

        public EntityView Create(EntityDefinition entityDefinition)
        {
            var prefab = entityDefinition.VisualModel.BasePrefab != null
                ? entityDefinition.VisualModel.BasePrefab
                : _defaultPrefabReferences.EntityView;
            var entityView = _poolService.Take(prefab, Vector3.zero, Quaternion.identity);
            entityView.name = $"{entityDefinition.VisualModel} ({++_entitiesCounter})";

            var unitsController = _unitsControllerFactory.Create(entityView.UnitsRoot, entityDefinition.VisualModel, entityDefinition.VariantName);

            entityView.Initialize(unitsController);

            return entityView;
        }
    }
}
