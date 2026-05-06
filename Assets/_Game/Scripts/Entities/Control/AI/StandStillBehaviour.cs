using Assets._Game.Scripts.Entities.Modules;

namespace Assets._Game.Scripts.Entities.Control.AI
{
    internal class StandStillBehaviour : IAiBehaviour
    {
        public BehaviourEvaluation Evaluate(Entity entity)
        {
            return new BehaviourEvaluation(0.01f, null);
        }

        public void Tick(Entity entity, IBehaviourContext context, float delta)
        {
            var intent = entity.GetModule<IntentModule>();
            intent.ClearAct();
            intent.ClearMove();
        }
    }
}
