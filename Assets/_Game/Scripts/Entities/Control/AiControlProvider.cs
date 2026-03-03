using Assets._Game.Scripts.Entities.Modules;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Control
{
    public sealed class AiControlProvider : IControlProvider
    {
        public ControlPriority Priority => ControlPriority.BaseAI;
        public ControlMask Mask => ControlMask.All;
        public bool IsActive => true;

        private Vector2 _moveTarget = new(-3, 4);
        private float _stopRadius = 0.1f;

        public void Tick(Entity entity, float delta)
        {
            if (!entity.TryGetModule(out IntentModule intent))
                return;

            if (!entity.TryGetModule(out SpatialModule spatial))
                return;

            var moveDirection = _moveTarget - spatial.Position;

            if (moveDirection.sqrMagnitude <= _stopRadius * _stopRadius)
            {
                _moveTarget = new Vector2(UnityEngine.Random.Range(-3f, 3f), UnityEngine.Random.Range(-5f, 5f));
                return;
            }

            intent.SetMove(new(moveDirection));
        }
    }

    [Serializable]
    public sealed class AiControlProviderData : ControlProviderData
    {
        public override IControlProvider CreateInstance() => new AiControlProvider();
    }
}
