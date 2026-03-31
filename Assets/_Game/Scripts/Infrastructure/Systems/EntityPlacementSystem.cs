using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Querying;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class EntityPlacementSystem : EntitySystemBase
    {
        protected override EntityQuery EntityQuery { get; } = new(requiredModules: new Type[] { typeof(SpatialModule) });

        public EntityPlacementSystem(EntityRepository repository) : base(repository)
        {
            TrackEntityEvent<EntityPlacementRequest>(OnEntityPlacementRequested);
        }

        private void OnEntityPlacementRequested(EntityPlacementRequest request)
        {
            var spatialModule = request.Entity.GetModule<SpatialModule>();
            spatialModule.RequestSetPosition(request.Position);
        }
    }

    public readonly struct EntityPlacementRequest : IEntityEvent
    {
        public readonly Vector2 Position;

        public Entity Entity { get; }

        public EntityPlacementRequest(Entity entity, Vector2 position)
        {
            Entity = entity;
            Position = position;
        }
    }
}
