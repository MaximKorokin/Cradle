using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using System;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class ResetSystem : SystemBase
    {
        private readonly EntityRepository _entityRepository;

        public ResetSystem(
            IGlobalEventBus globalEventBus,
            EntityRepository entityRepository) : base(globalEventBus)
        {
            _entityRepository = entityRepository;

            TrackGlobalEvent<ResetEntityModulesRequest>(OnResetEntityModulesRequested);
            TrackGlobalEvent<ResetAllEntityModulesRequest>(OnResetAllEntityModulesRequested);
            TrackGlobalEvent<ResetEntityModuleRequest>(OnResetEntityModuleRequested);
        }

        private void OnResetEntityModulesRequested(ResetEntityModulesRequest r)
        {
            if (r.Entity == null) return;
            ResetEntityModules(r.Entity);
        }

        private void OnResetAllEntityModulesRequested(ResetAllEntityModulesRequest r)
        {
            foreach (var entity in _entityRepository.All)
            {
                ResetEntityModules(entity);
            }
        }

        private void OnResetEntityModuleRequested(ResetEntityModuleRequest request)
        {
            if (request.Entity == null || request.ModuleType == null) return;

            var module = request.Entity.GetModule(request.ModuleType);
            if (module is IResettableModule resettable)
            {
                resettable.Reset();
            }
        }

        private static void ResetEntityModules(Entity entity)
        {
            foreach (var module in entity.Modules)
            {
                if (module is IResettableModule resettable)
                {
                    resettable.Reset();
                }
            }
        }
    }

    public readonly struct ResetEntityModulesRequest : IGlobalEvent
    {
        public readonly Entity Entity;
        public ResetEntityModulesRequest(Entity entity) => Entity = entity;
    }

    public readonly struct ResetAllEntityModulesRequest : IGlobalEvent { }

    public readonly struct ResetEntityModuleRequest : IGlobalEvent
    {
        public readonly Entity Entity;
        public readonly Type ModuleType;

        public ResetEntityModuleRequest(Entity entity, Type moduleType)
        {
            Entity = entity;
            ModuleType = moduleType;
        }
    }
}
