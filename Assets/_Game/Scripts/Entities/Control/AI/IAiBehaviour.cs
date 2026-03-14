using Assets._Game.Scripts.Entities.Interactions.Action;

namespace Assets._Game.Scripts.Entities.Control.AI
{
    public interface IAiBehaviour
    {
        ActionEvaluation Evaluate(Entity entity);
        void Tick(Entity entity, ActionContext context, float delta);
    }

    public readonly struct ActionEvaluation
    {
        public readonly float Score;
        public readonly ActionContext Context;

        public ActionEvaluation(float score, ActionContext context)
        {
            Score = score;
            Context = context;
        }
    }

    public readonly struct ActionContext
    {
        public readonly ActionInstance ActionInstance;
        public readonly Entity Target;

        public ActionContext(ActionInstance actionInstance, Entity target)
        {
            ActionInstance = actionInstance;
            Target = target;
        }
    }
}
