using Assets._Game.Scripts.Entities.Control.Intents;
using Assets._Game.Scripts.Entities.Interactions;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class IntentModule : EntityModuleBase
    {
        public MoveIntent Move { get; private set; }
        public AimIntent Aim { get; private set; }
        public InteractIntent Interact { get; private set; }

        public void SetMove(Vector2 direction, float speedMultiplier = 1f)
        {
            Move = new MoveIntent(direction, speedMultiplier);
        }

        public void SetAim(Vector2 direction)
        {
            Aim = new AimIntent(direction);
        }

        public void SetInteract(InteractionDefinition interactionDefinition, UseKind kind, Entity target, Vector2 point, bool hasPoint)
        {
            Interact = new InteractIntent(interactionDefinition, kind, target, point, hasPoint);
        }
    }
}
