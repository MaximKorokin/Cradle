using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Control;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.Shared.Utils;
using VContainer;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class ControlSystem : ReactiveEntitySystemBase, ITickSystem
    {
        private readonly IObjectResolver _resolver;

        protected override EntityQuery EntityQuery { get; } =
            new EntityQuery(
                RestrictionState.Disabled | RestrictionState.Dead,
                new[] { typeof(ControlModule) }
            );

        public ControlSystem(EntityRepository repository, IObjectResolver resolver) : base(repository)
        {
            _resolver = resolver;
        }

        public void Tick(float delta)
        {
            foreach (var entity in EnumerateEntities())
            {
                TickEntity(entity, delta);
            }
        }

        private void TickEntity(Entity entity, float delta)
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

        protected override void OnTrack(Entity entity)
        {
            entity.Subscribe<StatusEffectChangedEvent>(OnStatusEffectChanged);
        }

        protected override void OnUntrack(Entity entity)
        {
            entity.Subscribe<StatusEffectChangedEvent>(OnStatusEffectChanged);
        }

        private void OnStatusEffectChanged(StatusEffectChangedEvent e)
        {
            if (e.Kind == StatusEffectChangeKind.Added && e.StatusEffect.Definition.ControlProvider != null)
            {
                var controlModule = e.Entity.GetModule<ControlModule>();
                controlModule.AddProvider(e.StatusEffect.Definition.ControlProvider.CreateInstance(_resolver));
            }
        }
    }
}
