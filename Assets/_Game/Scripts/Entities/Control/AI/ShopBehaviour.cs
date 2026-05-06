using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Querying;
using Assets._Game.Scripts.UI.Systems;

namespace Assets._Game.Scripts.Entities.Control.AI
{
    public sealed class ShopBehaviour : IAiBehaviour
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly IEntitySensor _entitySensor;

        public ShopBehaviour(IGlobalEventBus globalEventBus, IEntitySensor entitySensor)
        {
            _globalEventBus = globalEventBus;
            _entitySensor = entitySensor;
        }

        public BehaviourEvaluation Evaluate(Entity entity)
        {
            if (entity.TryGetModule<ShopModule>(out var shopModule) &&
                _entitySensor.TryGetFirstInRange(entity, shopModule.Radius, Faction.FactionRelation.Ally, default, out var foundEntity))
            {
                var context = new TargetBehaviourContext(foundEntity);
                return new BehaviourEvaluation(1, context);
            }

            return new BehaviourEvaluation(0, null);
        }

        public void Tick(Entity entity, IBehaviourContext context, float delta)
        {
            if (entity.TryGetModule<ShopModule>(out var shopModule) && context is TargetBehaviourContext targetContext)
            {
                _globalEventBus.Publish(InteractionPromptViewRequest.ShowRequest(shopModule.Definition.ShopName, "Open", () => _globalEventBus.Publish(new ShopWindowOpenRequest(entity.Id, targetContext.Target.Id))));
            }
        }
    }
}
