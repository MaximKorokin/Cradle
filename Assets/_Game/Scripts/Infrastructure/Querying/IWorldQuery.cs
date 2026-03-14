using Assets._Game.Scripts.Entities;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Querying
{
    public interface IWorldQuery
    {
        bool HasLineOfSight(Vector2 from, Vector2 to);
        bool IsReachable(Vector2 from, Vector2 to);
        int GetEntitiesInRange(Vector2 point, float radius, Entity[] entities);
    }

    public sealed class UnityWorldQuery : IWorldQuery
    {
        private readonly Collider2D[] _buffer = new Collider2D[256];
        private readonly LayerMask _obstacleMask = 0;
        private readonly LayerMask _entityMask = ~0;

        public bool HasLineOfSight(Vector2 from, Vector2 to)
        {
            return !Physics2D.Linecast(from, to, _obstacleMask);
        }

        public bool IsReachable(Vector2 from, Vector2 to)
        {
            // NavMesh / grid / etc
            return true;
        }

        public int GetEntitiesInRange(Vector2 point, float radius, Entity[] entities)
        {
            var count = Physics2D.OverlapCircle(point, radius, new() { layerMask = _entityMask, useTriggers = true }, _buffer);

            var entitiesFound = 0;
            for (int i = 0; i < count; i++)
            {
                if (_buffer[i].TryGetComponent<EntityView>(out var view))
                {
                    entities[entitiesFound] = view.Entity;
                    entitiesFound++;
                }
            }
            return entitiesFound;
        }
    }
}
