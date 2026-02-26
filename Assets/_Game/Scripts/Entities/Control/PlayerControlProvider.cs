using Assets._Game.Scripts.Entities.Modules;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Control
{
    public sealed class PlayerControlProvider : IControlProvider
    {
        public ControlPriority Priority => ControlPriority.BasePlayer;
        public ControlMask Mask => ControlMask.All;
        public bool IsActive => true;

        private Vector2 _move;

        public void SetInput(Vector2 move)
        {
            _move = move;
        }

        public void Tick(Entity entity, float delta)
        {
            if (entity.TryGetModule(out IntentModule intent))
            {
                intent.SetMove(_move, 1f);
            }
        }
    }
}
