using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Shared.Extensions;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public abstract class EntitySystemBase : SystemBase
    {
        protected readonly EntityRepository EntityRepository;
        private readonly Entity[] _buffer = new Entity[2048];

        protected abstract EntityQuery EntityQuery { get; }

        protected EntitySystemBase(EntityRepository repository)
        {
            EntityRepository = repository;
        }

        protected virtual IEnumerable<Entity> EnumerateEntities()
        {
            var count = EntityRepository.CopyAllTo(_buffer);
            for (int i = 0; i < count; i++)
            {
                var entity = _buffer[i];
                if (EntityQuery.Match(entity))
                    yield return entity;
            }
        }
    }
}
