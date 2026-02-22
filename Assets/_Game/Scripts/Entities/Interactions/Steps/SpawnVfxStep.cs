namespace Assets._Game.Scripts.Entities.Interactions.Steps
{
    public sealed class SpawnVfxStep : IInteractionStep
    {
        private readonly string _vfxId;
        private bool _done;

        public SpawnVfxStep(string vfxId) => _vfxId = vfxId;

        public void Start(in InteractionContext context) => _done = false;

        public StepStatus Tick(in InteractionContext context, float delta)
        {
            if (_done) return StepStatus.Completed;

            var spawnPoint = context.Point;
            //context.GlobalBus.Publish(new SpawnVfxRequested(_vfxId, point));

            _done = true;
            return StepStatus.Completed;
        }

        public void Cancel(in InteractionContext context) { }
    }
}
