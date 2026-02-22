using Assets._Game.Scripts.Entities.Interaction.Steps;

namespace Assets._Game.Scripts.Entities.Interaction
{
    public sealed class InteractionInstance
    {
        private readonly IInteractionStep _root;
        private readonly InteractionContext _context;
        private bool _started;

        public InteractionInstance(IInteractionStep root, in InteractionContext context)
        {
            _root = root;
            _context = context;
        }

        public void Start()
        {
            if (_started) return;
            _started = true;
            _root.Start(_context);
        }

        /// <summary>
        /// return true if the interaction has finished (either completed or failed), false if it is still running
        /// </summary>
        /// <param name="dt"></param>
        /// <returns>true = finished</returns>
        public bool Tick(float dt)
        {
            if (!_started) Start();
            var step = _root.Tick(_context, dt);
            return step is StepStatus.Completed or StepStatus.Failed;
        }

        public void Cancel()
        {
            _root.Cancel(_context);
        }
    }
}
