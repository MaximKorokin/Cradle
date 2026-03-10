using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Shared.Extensions;
using System;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public abstract class EntitySystemBase : SystemBase
    {
        protected readonly EntityRepository EntityRepository;
        private Entity[] _buffer = new Entity[256];

        protected abstract EntityQuery EntityQuery { get; }

        private readonly List<Action<Entity>> _subscribe = new();
        private readonly List<Action<Entity>> _unsubscribe = new();

        protected EntitySystemBase(EntityRepository repository)
        {
            EntityRepository = repository;

            EntityRepository.Added += OnEntityAdded;
            EntityRepository.Removed += OnEntityRemoved;

            IterateAllEntities(SubscribeEntity);
        }

        // ------------------------ Reacting ------------------------
        protected void TrackEntityEvent<T>(Action<T> handler) where T : struct, IEntityEvent
        {
            void Callback(T evt)
            {
                var entity = evt.Entity;

                if (!EntityQuery.Match(entity))
                    return;

                handler(evt);
            }

            _subscribe.Add(e => e.Subscribe<T>(Callback));
            _unsubscribe.Add(e => e.Unsubscribe<T>(Callback));

            IterateAllEntities(entity => entity.Subscribe<T>(Callback));
        }

        protected virtual void OnEntityAdded(Entity entity)
        {
            SubscribeEntity(entity);
        }

        protected virtual void OnEntityRemoved(Entity entity)
        {
            UnsubscribeEntity(entity);
        }

        private void SubscribeEntity(Entity entity)
        {
            for (int i = 0; i < _subscribe.Count; i++)
            {
                _subscribe[i](entity);
            }
        }

        private void UnsubscribeEntity(Entity entity)
        {
            for (int i = 0; i < _unsubscribe.Count; i++)
            {
                _unsubscribe[i](entity);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            EntityRepository.Added -= OnEntityAdded;
            EntityRepository.Removed -= OnEntityRemoved;

            IterateAllEntities(UnsubscribeEntity);
        }

        // ------------------------ Iterating ------------------------
        protected void IterateAllEntities(Action<Entity> callback)
        {
            var count = CopySnapshot();

            for (var i = 0; i < count; i++)
            {
                callback(_buffer[i]);
            }
        }

        protected void IterateMatchingEntities(Action<Entity> callback)
        {
            var count = CopySnapshot();

            for (var i = 0; i < count; i++)
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
                newSize *= 2;

            _buffer = new Entity[newSize];
        }
    }
}
