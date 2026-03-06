using Assets._Game.Scripts.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure
{
    public interface IWorldQuery
    {
        bool HasLineOfSight(Vector2 from, Vector2 to);
        bool IsReachable(Vector2 from, Vector2 to);
        IEnumerable<Entity> GetEntitiesInRange(Vector2 point, float radius);
    }

    public sealed class UnityWorldQuery : IWorldQuery
    {
        private readonly Collider2D[] _buffer = new Collider2D[32];
        private readonly LayerMask _obstacleMask = 0;
        private readonly LayerMask _entityMask = ~0;

        //public UnityWorldQuery(LayerMask obstacleMask)
        //{
        //    _obstacleMask = obstacleMask;
        //}

        public bool HasLineOfSight(Vector2 from, Vector2 to)
        {
            return !Physics2D.Linecast(from, to, _obstacleMask);
        }

        public bool IsReachable(Vector2 from, Vector2 to)
        {
            // NavMesh / grid / etc
            return true;
        }

        public IEnumerable<Entity> GetEntitiesInRange(Vector2 point, float radius)
        {
            var count = Physics2D.OverlapCircle(point, radius, new() { layerMask = _entityMask }, _buffer);

            for (int i = 0; i < count; i++)
            {
                if (_buffer[i].TryGetComponent<EntityView>(out var view))
                    yield return view.Entity;
            }
        }
    }
}
