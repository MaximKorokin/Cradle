using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Storage;
using Assets._Game.Scripts.Shared.Extensions;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class DespawnSystem : EntitySystemBase, ITickSystem
    {
        private readonly IGlobalEventBus _globalEventBus;

        protected override EntityQuery EntityQuery => new(RestrictionState.Disabled, new[] { typeof(DespawnModule) });

        public DespawnSystem(EntityRepository repository, IGlobalEventBus globalEventBus) : base(repository)
        {
            _globalEventBus = globalEventBus;

            _globalEventBus.Subscribe<EntityDiedEvent>(OnEntityDied);
        }

        public override void Dispose()
        {
            base.Dispose();

            _globalEventBus.Unsubscribe<EntityDiedEvent>(OnEntityDied);
        }

        public void Tick(float delta)
        {
            IterateMatchingEntities(TickEntity);
        }

        private void TickEntity(Entity entity)
        {
            if (entity.GetModule<DespawnModule>().IsExpired)
            {
                _globalEventBus.Publish(new DespawnEntityViewRequestEvent(entity));
                // For now entity does not exist if it does not have view
                // There will be a big TODO in the future if this will change
                EntityRepository.Remove(((IEntry)entity).Id);
            }
        }

        protected override void OnEntityAdded(Entity entity)
        {
            base.OnEntityAdded(entity);

            if (entity.TryGetModule<DespawnModule>(out var module))
            {
                if (module.Trigger == DespawnCounterStartTrigger.OnSpawn) module.StartDespawnTime = Time.time;
                else if (module.Trigger == DespawnCounterStartTrigger.OnDeath) module.StartDespawnTime = null;
            }
        }

        private void OnEntityDied(EntityDiedEvent e)
        {
            if (e.Victim.TryGetModule<DespawnModule>(out var module))
            {
                if (module.Trigger == DespawnCounterStartTrigger.OnDeath)
                {
                    module.StartDespawnTime = Time.time;
                }
            }
        }
    }
}
