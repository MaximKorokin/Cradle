using Assets._Game.Scripts.Entities;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public abstract class ReactiveEntitySystemBase : SystemBase
    {
        private readonly EntityRepository _repository;
        private readonly HashSet<Entity> _tracked = new();

        protected ReactiveEntitySystemBase(EntityRepository repository)
        {
            _repository = repository;

            _repository.Added += OnEntityAdded;
            _repository.Removed += OnEntityRemoved;

            foreach (var entity in _repository.All)
            {
                OnEntityAdded(entity);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            _repository.Added -= OnEntityAdded;
            _repository.Removed -= OnEntityRemoved;
        }

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
            if (!Filter(entity)) return;

            if (_tracked.Add(entity))
            {
                OnTrack(entity);
            }
        }

        protected abstract bool Filter(Entity entity);

        protected abstract void OnTrack(Entity entity);

        protected abstract void OnUntrack(Entity entity);
    }
}
