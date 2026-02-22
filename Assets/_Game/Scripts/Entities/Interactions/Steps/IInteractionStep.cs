namespace Assets._Game.Scripts.Entities.Interactions.Steps
{
    public enum StepStatus { Running, Completed, Failed }

    public interface IInteractionStep
    {
        void Start(in InteractionContext context);

        StepStatus Tick(in InteractionContext context, float delta);

        void Cancel(in InteractionContext context);
    }
}
