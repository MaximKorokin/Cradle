using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Querying;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class EntityPlacementSystem : EntitySystemBase
    {
        protected override EntityQuery EntityQuery { get; } = new(requiredModules: new Type[] { typeof(SpatialModule) });

        public EntityPlacementSystem(
            IGlobalEventBus globalEventBus,
            EntityRepository repository) : base(globalEventBus, repository)
        {
            TrackEntityEvent<EntityRepositionRequest>(OnEntityRepositionRequested);
        }

        private void OnEntityRepositionRequested(Entity entity, EntityRepositionRequest request)
        {
            var spatialModule = entity.GetModule<SpatialModule>();
            spatialModule.RequestSetPosition(request.Position);
        }
    }

    public readonly struct EntityRepositionRequest : IEntityEvent
    {
        public readonly Vector2 Position;

        public EntityRepositionRequest(Vector2 position)
        {
            Position = position;
        }
    }
}
