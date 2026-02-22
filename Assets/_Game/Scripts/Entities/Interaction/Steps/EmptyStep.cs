namespace Assets._Game.Scripts.Entities.Interaction.Steps
{
    public class EmptyStep : IInteractionStep
    {
        public void Start(in InteractionContext context) { }

        public StepStatus Tick(in InteractionContext context, float delta) => StepStatus.Completed;

        public void Cancel(in InteractionContext context) { }
    }
}
