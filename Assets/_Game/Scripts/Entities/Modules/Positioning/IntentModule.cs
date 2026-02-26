using Assets._Game.Scripts.Entities.Control.Intents;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class IntentModule : EntityModuleBase
    {
        public MoveIntent Move { get; private set; }
        public AimIntent Aim { get; private set; }
        public InteractIntent Interact { get; private set; }

        public void SetMove(MoveIntent moveIntent)
        {
            Move = moveIntent;
        }

        public void SetAim(AimIntent aimIntent)
        {
            Aim = aimIntent;
        }

        public void SetInteract(InteractIntent interactIntent)
        {
            Interact = interactIntent;
        }
    }
}
