using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.Shared.Extensions
{
    public static class EntityExtensions
    {
        public static Vector2 GetPosition(this Entity entity)
        {
            if (entity.TryGetModule(out SpatialModule transformModule))
            {
                return transformModule.Position;
            }
            throw new InvalidOperationException($"Entity {entity} does not have a {typeof(SpatialModule)}.");
        }
    }
}
