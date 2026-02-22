using Assets._Game.Scripts.Entities.Modules;

namespace Assets._Game.Scripts.Entities.Control
{
    public sealed class AiControlProvider : IControlProvider
    {
        public ControlPriority Priority => ControlPriority.BaseAI;
        public bool IsActive => true;

        public void Tick(Entity entity, float delta)
        {
            // todo: implement ai control
            if (entity.TryGetModule(out IntentModule intent))
            {
                intent.StopMove();
            }
        }
    }
}
