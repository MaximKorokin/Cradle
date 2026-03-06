namespace Assets._Game.Scripts.Entities.Control.AI
{
    public interface IAiBehaviour
    {
        void Initialize(Entity entity);
        float Evaluate();
        void Execute(float delta);
    }
}
