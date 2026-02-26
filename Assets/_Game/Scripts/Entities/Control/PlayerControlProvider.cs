using Assets._Game.Scripts.Entities.Control.Intents;
using Assets._Game.Scripts.Entities.Modules;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Control
{
    public sealed class PlayerControlProvider : IControlProvider
    {
        public ControlPriority Priority => ControlPriority.BasePlayer;
        public ControlMask Mask => ControlMask.All;
        public bool IsActive => true;

        private bool _hasMoveTarget;
        private Vector2 _moveTarget;
        private float _stopRadius;

        public void SetMoveTarget(Vector2 moveTarget, float stopRadius = 0.15f)
        {
            _hasMoveTarget = true;
            _moveTarget = moveTarget;
            _stopRadius = stopRadius;
        }

        public void Tick(Entity entity, float delta)
        {
            if (!_hasMoveTarget)
                return;

            if (!entity.TryGetModule(out IntentModule intent))
                return;

            if (!entity.TryGetModule(out SpatialModule spatial))
                return;

            var moveDirection = _moveTarget - spatial.Position;

            if (moveDirection.sqrMagnitude <= _stopRadius * _stopRadius)
            {
                _hasMoveTarget = false;
                intent.SetMove(MoveIntent.None);
                return;
            }

            intent.SetMove(new(moveDirection));
        }
    }
}
