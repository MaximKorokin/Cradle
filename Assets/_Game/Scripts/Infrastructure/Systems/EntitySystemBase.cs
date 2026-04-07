using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Infrastructure.Querying;
using System;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public abstract class EntitySystemBase : SystemBase
    {
        protected readonly EntityRepository EntityRepository;

        private Entity[] _buffer = new Entity[256];
        private readonly List<Action<Entity, List<IDisposable>>> _eventBinders = new();
        private readonly Dictionary<Entity, List<IDisposable>> _subscriptionsByEntity = new();

        protected abstract EntityQuery EntityQuery { get; }

        protected EntitySystemBase(EntityRepository repository)
        {
            EntityRepository = repository;

            EntityRepository.Added += OnEntityAdded;
            EntityRepository.Removed += OnEntityRemoved;

            IterateAllEntities(RegisterEntity);
        }

        protected void TrackEntityEvent<T>(Action<Entity, T> handler)
            where T : struct, IEntityEvent
        {
            void Bind(Entity entity, List<IDisposable> subscriptions)
            {
                var subscription = entity.Subscribe<T>(evt =>
                {
                    if (!EntityQuery.Match(entity))
                        return;

                    handler(entity, evt);
                });

                subscriptions.Add(subscription);
            }

            _eventBinders.Add(Bind);

            foreach (var pair in _subscriptionsByEntity)
            {
                Bind(pair.Key, pair.Value);
            }
        }

        protected virtual void OnEntityAdded(Entity entity)
        {
            RegisterEntity(entity);
        }

        protected virtual void OnEntityRemoved(Entity entity)
        {
            UnregisterEntity(entity);
        }

        private void RegisterEntity(Entity entity)
        {
            if (_subscriptionsByEntity.ContainsKey(entity))
                return;

            var subscriptions = new List<IDisposable>(4);
            _subscriptionsByEntity.Add(entity, subscriptions);

            for (int i = 0; i < _eventBinders.Count; i++)
            {
                _eventBinders[i](entity, subscriptions);
            }
        }

        private void UnregisterEntity(Entity entity)
        {
            if (!_subscriptionsByEntity.TryGetValue(entity, out var subscriptions))
                return;

            for (int i = 0; i < subscriptions.Count; i++)
            {
                subscriptions[i].Dispose();
            }

            _subscriptionsByEntity.Remove(entity);
        }

        public override void Dispose()
        {
            EntityRepository.Added -= OnEntityAdded;
            EntityRepository.Removed -= OnEntityRemoved;

            foreach (var pair in _subscriptionsByEntity)
            {
                var subscriptions = pair.Value;
                for (int i = 0; i < subscriptions.Count; i++)
                {
                    subscriptions[i].Dispose();
                }
            }

            _subscriptionsByEntity.Clear();
            _eventBinders.Clear();

            base.Dispose();
        }

        protected void IterateAllEntities(Action<Entity> callback)
        {
            var count = CopySnapshot();

            for (int i = 0; i < count; i++)
            {
                callback(_buffer[i]);
            }
        }

        protected void IterateMatchingEntities(Action<Entity> callback)
        {
            var count = CopySnapshot();

            for (int i = 0; i < count; i++)
            {
                var entity = _buffer[i];

                if (EntityQuery.Match(entity))
                    callback(entity);
            }
        }

        private int CopySnapshot()
        {
            EnsureCapacity(EntityRepository.All.Count);
            return EntityRepository.CopyAllTo(_buffer);
        }

        private void EnsureCapacity(int required)
        {
            if (_buffer.Length >= required)
                return;

            var newSize = _buffer.Length;
            while (newSize < required)
            {
                newSize *= 2;
            }

            _buffer = new Entity[newSize];
        }
    }
}
