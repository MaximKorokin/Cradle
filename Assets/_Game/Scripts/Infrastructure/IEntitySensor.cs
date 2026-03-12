using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Faction;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Shared.Extensions;

namespace Assets._Game.Scripts.Infrastructure
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
            out Entity entity);

        bool HasAnyInRange(
            Entity self,
            float radius,
            FactionRelation relation,
            EntityQuery query);
    }

    public sealed class EntitySensor : IEntitySensor
    {
        private const int BufferSize = 32;

        private readonly IWorldQuery _worldQuery;
        private readonly FactionRelationResolver _relationResolver;

        private readonly Entity[] _buffer = new Entity[BufferSize];

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

            var count = _worldQuery.GetEntitiesInRange(position, radius, _buffer);

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
            out Entity entity)
        {
            var result = false;
            var minDistance = float.MaxValue;
            entity = null;

            var position = self.GetModule<SpatialModule>().Position;

            var count = _worldQuery.GetEntitiesInRange(position, radius, _buffer);

            for (var i = 0; i < count; i++)
            {
                var candidate = _buffer[i];
                if (candidate == null) continue;
                if (candidate == self) continue;
                if (_relationResolver.GetRelation(candidate, self) != relation) continue;
                if (!query.Match(candidate)) continue;

                var distance = (candidate.GetModule<SpatialModule>().Position - position).magnitude;
                if (distance < minDistance)
                {
                    entity = candidate;
                    minDistance = distance;
                    result = true;
                }
            }

            return result;
        }
    }
}
