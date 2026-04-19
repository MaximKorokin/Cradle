using Assets._Game.Scripts.Entities.Modules;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Control
{
    public sealed class PlayerControlProvider : ControlProviderBase
    {
        public override ControlPriority Priority => ControlPriority.BasePlayer;
        public override ControlMask Mask => _hasMoveTarget ? ControlMask.All : ControlMask.None;
        public override bool IsActive => true;

        private bool _hasMoveTarget;
        private Vector2 _moveTarget;
        private float _stopRadius;

        public void SetMoveTarget(Vector2 moveTarget, float stopRadius = 0.15f)
        {
            _hasMoveTarget = true;
            _moveTarget = moveTarget;
            _stopRadius = stopRadius;
        }

        public override void Tick(float delta)
        {
            if (!_hasMoveTarget)
                return;

            if (!Entity.TryGetModule(out IntentModule intent))
                return;

            if (!Entity.TryGetModule(out SpatialModule spatial))
                return;

            var moveDirection = _moveTarget - spatial.Position;

            if (moveDirection.sqrMagnitude <= _stopRadius * _stopRadius)
            {
                _hasMoveTarget = false;
                intent.ClearMove();
                return;
            }

            intent.SetMove(new(moveDirection));
            intent.ClearAct();
        }

        public override void Reset()
        {
            base.Reset();

            _hasMoveTarget = false;
            _moveTarget = Vector2.zero;
        }
    }
}
