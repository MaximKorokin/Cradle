using Assets._Game.Scripts.Entities;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public abstract class ReactiveEntitySystemBase : EntitySystemBase
    {
        private readonly HashSet<Entity> _tracked = new();

        protected ReactiveEntitySystemBase(EntityRepository repository) : base(repository)
        {
            EntityRepository.Added += OnEntityAdded;
            EntityRepository.Removed += OnEntityRemoved;

            foreach (var entity in EnumerateEntities())
            {
                OnEntityAdded(entity);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            EntityRepository.Added -= OnEntityAdded;
            EntityRepository.Removed -= OnEntityRemoved;
        }

        protected override IEnumerable<Entity> EnumerateEntities() => _tracked;

        private void OnEntityAdded(Entity entity)
        {
            TryTrack(entity);
        }

        private void OnEntityRemoved(Entity entity)
        {
            if (_tracked.Remove(entity))
            {
                OnUntrack(entity);
            }
        }

        private void TryTrack(Entity entity)
        {
            if (!EntityQuery.Match(entity)) return;

            if (_tracked.Add(entity))
            {
                OnTrack(entity);
            }
        }

        protected abstract void OnTrack(Entity entity);

        protected abstract void OnUntrack(Entity entity);
    }
}
