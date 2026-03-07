using Assets._Game.Scripts.Entities.Faction;
using Assets._Game.Scripts.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.Shared.Extensions
{
    public static class WorldQueryExtensions
    {
        public static IEnumerable<Entity> GetEntitiesInRange(this IWorldQuery worldQuery, Vector2 point, float radius, Entity self, FactionRelation relation, FactionRelationResolver relationResolver)
        {
            return worldQuery.GetEntitiesInRange(point, radius).Where(e => relationResolver.GetRelation(e, self) == relation);
        }
    }
}
