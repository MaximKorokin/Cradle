using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Shared.Extensions
{
    public static class EntityQueryExtensions
    {
        public static IEnumerable<Entity> Query(this IEnumerable<Entity> entities, EntityQuery query)
        {
            foreach (var entity in entities)
            {
                var state = entity.GetModule<RestrictionStateModule>();

                if ((state.State & query.ExcludeStates) != 0)
                    continue;

                yield return entity;
            }
        }
    }

    public struct EntityQuery
    {
        public RestrictionState ExcludeStates;

        public EntityQuery(RestrictionState _excludeStates)
        {
            ExcludeStates = _excludeStates;
        }
    }
}
