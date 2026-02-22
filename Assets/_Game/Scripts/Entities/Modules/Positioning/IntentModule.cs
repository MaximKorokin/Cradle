using Assets._Game.Scripts.Entities.Control;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class IntentModule : EntityModuleBase
    {
        public MoveIntent Move { get; private set; }

        public void SetMove(Vector2 direction, float speedMultiplier = 1f)
        {
            Move = new MoveIntent(direction, speedMultiplier);
        }

        public void StopMove()
        {
            Move = new MoveIntent(Vector2.zero, 1f);
        }
    }
}
