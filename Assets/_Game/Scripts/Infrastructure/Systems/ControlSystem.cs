using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Control;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.Shared.Utils;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class ControlSystem : ReactiveEntitySystemBase
    {
        protected override EntityQuery EntityQuery => new(RestrictionState.Disabled | RestrictionState.Dead);

        public ControlSystem(EntityRepository repository, DispatcherService dispatcher) : base(repository, dispatcher)
        {
            TickAction += Tick;
        }

        public void Tick(Entity entity, float delta)
        {
            var controlModule = entity.GetModule<ControlModule>();

            for (int i = 0; i < controlModule.Providers.Count; i++)
            {
                if (!controlModule.Providers[i].IsActive)
                {
                    controlModule.RemoveProvider(controlModule.Providers[i]);
                }
            }

            OptimizedOperationUtils.CleanTillNull(controlModule.RawControlProviders);
            OptimizedOperationUtils.CleanTillNull(controlModule.UniqueControlProviders);

            controlModule.RawControlProviders[0] = SelectBestControlProvider(controlModule, ControlMask.Move);
            controlModule.RawControlProviders[1] = SelectBestControlProvider(controlModule, ControlMask.Aim);
            controlModule.RawControlProviders[2] = SelectBestControlProvider(controlModule, ControlMask.Interact);

            OptimizedOperationUtils.CollectUnique(controlModule.RawControlProviders, controlModule.UniqueControlProviders);

            for (int i = 0; i < controlModule.UniqueControlProviders.Length; i++)
            {
                if (controlModule.UniqueControlProviders[i] == null) break;

                controlModule.UniqueControlProviders[i].Tick(delta);
            }
        }

        private static IControlProvider SelectBestControlProvider(ControlModule controlModule, ControlMask mask)
        {
            IControlProvider best = null;
            int bestPriority = int.MinValue;

            for (int i = 0; i < controlModule.Providers.Count; i++)
            {
                var provider = controlModule.Providers[i];
                if (!provider.IsActive) continue;
                if ((provider.Mask & mask) == 0) continue;

                var priority = (int)provider.Priority;
                if (priority > bestPriority || priority == bestPriority)
                {
                    best = provider;
                    bestPriority = priority;
                }
            }

            return best;
        }

        protected override bool Filter(Entity entity)
        {
            return entity.HasModule<ControlModule>();
        }

        protected override void OnTrack(Entity entity)
        {
            entity.Subscribe<OverrideControlRequestEvent>(OnOverrideControlRequested);
        }

        protected override void OnUntrack(Entity entity)
        {
            entity.Unsubscribe<OverrideControlRequestEvent>(OnOverrideControlRequested);
        }

        private void OnOverrideControlRequested(OverrideControlRequestEvent e)
        {
            e.Entity.GetModule<ControlModule>().AddProvider(e.ControlProvider);
        }
    }
}
