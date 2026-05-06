namespace Assets._Game.Scripts.Entities.Control.AI
{
    public interface IAiBehaviour
    {
        BehaviourEvaluation Evaluate(Entity entity);
        void Tick(Entity entity, IBehaviourContext context, float delta);
    }

    public readonly struct BehaviourEvaluation
    {
        public readonly float Score;
        public readonly IBehaviourContext Context;

        public BehaviourEvaluation(float score, IBehaviourContext context)
        {
            Score = score;
            Context = context;
        }
    }

    public interface IBehaviourContext
    {
    }

    public class TargetBehaviourContext : IBehaviourContext
    {
        public readonly Entity Target;

        public TargetBehaviourContext(Entity target)
        {
            Target = target;
        }
    }
}
