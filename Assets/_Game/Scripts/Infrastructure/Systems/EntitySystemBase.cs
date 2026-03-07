using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Shared.Extensions;
using System;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public abstract class EntitySystemBase : SystemBase
    {
        protected readonly EntityRepository EntityRepository;

        protected Action<Entity, float> TickAction;
        protected Action<Entity, float> FixedTickAction;

        protected abstract EntityQuery EntityQuery { get; }

        protected EntitySystemBase(EntityRepository repository, DispatcherService dispatcher) : base(dispatcher)
        {
            EntityRepository = repository;
        }

        protected override void OnTick(float delta)
        {
            base.OnTick(delta);

            if (TickAction == null) return;

            foreach (var entity in EnumerateEntities())
            {
                if (Filter(entity)) TickAction.Invoke(entity, delta);
            }
        }

        protected override void OnFixedTick(float delta)
        {
            base.OnFixedTick(delta);

            if (FixedTickAction == null) return;

            foreach (var entity in EnumerateEntities())
            {
                if (Filter(entity)) FixedTickAction.Invoke(entity, delta);
            }
        }

        protected abstract bool Filter(Entity entity);

        protected virtual IEnumerable<Entity> EnumerateEntities()
        {
            return EntityRepository.All.Query(EntityQuery);
        }
    }
}
