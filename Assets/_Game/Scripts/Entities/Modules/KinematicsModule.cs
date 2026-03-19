using UnityEngine;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class KinematicsModule : EntityModuleBase
    {
        public Vector2 Velocity { get; private set; }

        public void SetVelocity(Vector2 velocity)
        {
            Velocity = velocity;
        }
    }

    public sealed class KinematicsModuleFactory : IEntityModuleFactory
    {
        public EntityModuleBase Create(EntityDefinition entityDefinition)
        {
            return new KinematicsModule();
        }
    }
}
