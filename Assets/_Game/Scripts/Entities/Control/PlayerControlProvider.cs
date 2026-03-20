using Assets._Game.Scripts.Entities.Control.Intents;
using Assets._Game.Scripts.Entities.Modules;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Control
{
    public sealed class PlayerControlProvider : ControlProviderBase
    {
        public override ControlPriority Priority => ControlPriority.BasePlayer;
        public override ControlMask Mask => _hasMoveTarget ? ControlMask.All : ControlMask.None;
        public override bool IsActive => true;

        private Entity _entity;

        private bool _hasMoveTarget;
        private Vector2 _moveTarget;
        private float _stopRadius;

        public void SetMoveTarget(Vector2 moveTarget, float stopRadius = 0.15f)
        {
            _hasMoveTarget = true;
            _moveTarget = moveTarget;
            _stopRadius = stopRadius;
        }

        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);

            _entity = entity;
        }

        public override void Tick(float delta)
        {
            if (!_hasMoveTarget)
                return;

            if (!_entity.TryGetModule(out IntentModule intent))
                return;

            if (!_entity.TryGetModule(out SpatialModule spatial))
                return;

            var moveDirection = _moveTarget - spatial.Position;

            if (moveDirection.sqrMagnitude <= _stopRadius * _stopRadius)
            {
                _hasMoveTarget = false;
                intent.SetMove(default);
                return;
            }

            intent.SetMove(new(moveDirection));
            intent.SetAct(new(null, null, null));
        }
    }
}
