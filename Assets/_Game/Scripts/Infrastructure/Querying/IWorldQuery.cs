using Assets._Game.Scripts.Entities;
using System;
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
        private Collider2D[] _buffer = new Collider2D[256];
        private readonly LayerMask _obstacleMask = 0;
        private readonly LayerMask _entityMask = ~0;

        public bool HasLineOfSight(Vector2 from, Vector2 to)
        {
            return !Physics2D.Linecast(from, to, _obstacleMask);
        }

        public bool IsReachable(Vector2 from, Vector2 to)
        {
            return true;
        }

        public int GetEntitiesInRange(Vector2 point, float radius, Entity[] entities)
        {
            var count = OverlapCircleNonAlloc(point, radius);

            var found = 0;
            for (int i = 0; i < count && found < entities.Length; i++)
            {
                if (_buffer[i].TryGetComponent<EntityView>(out var view))
                    entities[found++] = view.Entity;
            }

            return found;
        }

        private int OverlapCircleNonAlloc(Vector2 point, float radius)
        {
            int count;

            while (true)
            {
                count = Physics2D.OverlapCircle(
                    point,
                    radius,
                    new ContactFilter2D
                    {
                        layerMask = _entityMask,
                        useLayerMask = true,
                        useTriggers = true
                    },
                    _buffer
                );

                if (count < _buffer.Length)
                    return count;

                Array.Resize(ref _buffer, _buffer.Length * 2);
            }
        }
    }
}
