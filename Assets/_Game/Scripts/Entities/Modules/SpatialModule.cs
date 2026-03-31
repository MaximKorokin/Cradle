using System;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class SpatialModule : EntityModuleBase
    {
        public Vector2 Position { get; private set; }
        public Vector2 Facing { get; private set; } = Vector2.right;

        public event Action<Vector2> PositionChanged;
        public event Action<Vector2> SetPositionRequested;

        public void SynchronizePosition(Vector2 position)
        {
            if (Position == position) return;

            Position = position;
            PositionChanged?.Invoke(position);
        }

        public void SetFacing(Vector2 facing)
        {
            if (facing.sqrMagnitude <= 0f)
                return;

            Facing = facing.normalized;
        }

        public void RequestSetPosition(Vector2 position)
        {
            SetPositionRequested?.Invoke(position);
        }
    }

    public sealed class SpatialModuleFactory : IEntityModuleFactory
    {
        public EntityModuleBase Create(EntityDefinition entityDefinition)
        {
            return new SpatialModule();
        }
    }
}
