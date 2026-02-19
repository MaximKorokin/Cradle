using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Units;
using VContainer;
using VContainer.Unity;

namespace Assets._Game.Scripts.Entities
{
    public sealed class EntityViewFactory
    {
        private static int _entitiesCounter = 0;

        private readonly IObjectResolver _resolver;
        private readonly UnitsControllerFactory _unitsControllerFactory;

        public EntityViewFactory(
            IObjectResolver resolver,
            UnitsControllerFactory unitsControllerFactory)
        {
            _resolver = resolver;
            _unitsControllerFactory = unitsControllerFactory;
        }

        public EntityView Create(EntityDefinition entityDefinition)
        {
            var entityView = _resolver.Instantiate(entityDefinition.VisualModel.BasePrefab);
            entityView.name = $"{entityDefinition.VisualModel} ({++_entitiesCounter})";

            entityView.UnitsController = _unitsControllerFactory.Create(
                entityView.UnitsRoot,
                entityView.UnitsAnimator,
                entityDefinition.VisualModel,
                entityDefinition.VariantName);

            return entityView;
        }
    }
}
