using UnityEngine;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class SpatialModule : EntityModuleBase
    {
        public Vector2 Position { get; private set; }
        public Vector2 Facing { get; private set; } = Vector2.right;

        public void SetPosition(Vector2 position)
        {
            Position = position;
        }

        public void SetFacing(Vector2 facing)
        {
            Facing = facing.sqrMagnitude > 0 ? facing.normalized : Facing;
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
