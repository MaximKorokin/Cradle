namespace Assets._Game.Scripts.Entities.Control.AI
{
    public abstract class AiBehaviourBase : IAiBehaviour
    {
        protected Entity Entity;

        public void Initialize(Entity entity)
        {
            Entity = entity;
            OnInitialize();
        }

        protected virtual void OnInitialize() { }

        public abstract float Evaluate();
        public abstract void Execute(float delta);
    }
}
