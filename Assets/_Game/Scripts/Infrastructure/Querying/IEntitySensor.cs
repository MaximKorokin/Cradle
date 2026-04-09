using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Faction;
using Assets._Game.Scripts.Entities.Modules;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Querying
{
    public interface IEntitySensor
    {
        bool TryGetFirstInRange(
            Entity self,
            float radius,
            FactionRelation relation,
            EntityQuery query,
            out Entity entity);

        bool TryGetNearestInRange(
            Entity self,
            float radius,
            FactionRelation relation,
            EntityQuery query,
            out Entity entity,
            out float distance);

        bool HasAnyInRange(
            Entity self,
            float radius,
            FactionRelation relation,
            EntityQuery query);
    }

    public sealed class EntitySensor : IEntitySensor
    {
        private readonly IWorldQuery _worldQuery;
        private readonly FactionRelationResolver _relationResolver;

        private Entity[] _buffer = new Entity[32];

        public EntitySensor(
            IWorldQuery worldQuery,
            FactionRelationResolver relationResolver)
        {
            _worldQuery = worldQuery;
            _relationResolver = relationResolver;
        }

        public bool HasAnyInRange(
            Entity self,
            float radius,
            FactionRelation relation,
            EntityQuery query)
        {
            return TryGetFirstInRange(self, radius, relation, query, out _);
        }

        public bool TryGetFirstInRange(
            Entity self,
            float radius,
            FactionRelation relation,
            EntityQuery query,
            out Entity entity)
        {
            entity = null;

            var position = self.GetModule<SpatialModule>().Position;
            var count = GetEntitiesWithResize(position, radius);

            for (var i = 0; i < count; i++)
            {
                var candidate = _buffer[i];
                if (candidate == null) continue;
                if (candidate == self) continue;
                if (_relationResolver.GetRelation(candidate, self) != relation) continue;
                if (!query.Match(candidate)) continue;

                entity = candidate;
                return true;
            }

            return false;
        }

        public bool TryGetNearestInRange(
            Entity self,
            float radius,
            FactionRelation relation,
            EntityQuery query,
            out Entity entity,
            out float distance)
        {
            var result = false;
            var minSqrDistance = float.MaxValue;
            entity = null;

            var position = self.GetModule<SpatialModule>().Position;
            var count = GetEntitiesWithResize(position, radius);

            for (var i = 0; i < count; i++)
            {
                var candidate = _buffer[i];
                if (candidate == null) continue;
                if (candidate == self) continue;
                if (_relationResolver.GetRelation(candidate, self) != relation) continue;
                if (!query.Match(candidate)) continue;

                var sqrDistance = (candidate.GetModule<SpatialModule>().Position - position).sqrMagnitude;

                if (sqrDistance < minSqrDistance)
                {
                    entity = candidate;
                    minSqrDistance = sqrDistance;
                    result = true;
                }
            }

            distance = Mathf.Sqrt(minSqrDistance);
            return result;
        }

        private int GetEntitiesWithResize(Vector2 position, float radius)
        {
            int count;

            while (true)
            {
                count = _worldQuery.GetEntitiesInRange(position, radius, _buffer);

                if (count < _buffer.Length)
                    return count;

                Array.Resize(ref _buffer, _buffer.Length * 2);
            }
        }
    }
}
