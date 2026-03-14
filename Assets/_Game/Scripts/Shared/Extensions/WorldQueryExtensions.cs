using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Faction;
using Assets._Game.Scripts.Infrastructure.Querying;
using UnityEngine;

namespace Assets._Game.Scripts.Shared.Extensions
{
    public static class WorldQueryExtensions
    {
        public static int GetEntitiesInRange(
            this IWorldQuery worldQuery,
            Vector2 point,
            float radius,
            Entity self,
            FactionRelation relation,
            FactionRelationResolver relationResolver,
            Entity[] entities)
        {
            var count = worldQuery.GetEntitiesInRange(point, radius, entities);

            var writeIndex = 0;

            for (var readIndex = 0; readIndex < count; readIndex++)
            {
                var entity = entities[readIndex];
                if (entity == null) continue;
                if (relationResolver.GetRelation(entity, self) != relation) continue;

                entities[writeIndex++] = entity;
            }

            for (var i = writeIndex; i < count; i++)
            {
                entities[i] = null;
            }

            return writeIndex;
        }
    }
}
