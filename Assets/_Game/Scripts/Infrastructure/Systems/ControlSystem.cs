using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Control;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Querying;
using Assets._Game.Scripts.Shared.Utils;
using UnityEngine;
using VContainer;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class ControlSystem : EntitySystemBase, ITickSystem
    {
        private readonly IObjectResolver _resolver;
        private readonly IGlobalEventBus _globalEventBus;

        protected override EntityQuery EntityQuery { get; } =
            new EntityQuery(
                RestrictionState.Disabled | RestrictionState.Dead,
                new[] { typeof(ControlModule) }
            );

        public ControlSystem(EntityRepository repository, IObjectResolver resolver, IGlobalEventBus globalEventBus) : base(repository)
        {
            _resolver = resolver;
            _globalEventBus = globalEventBus;

            _globalEventBus.Subscribe<EntityDiedEvent>(OnEntityDied);

            TrackEntityEvent<StatusEffectChangedEvent>(OnStatusEffectChanged);
            TrackEntityEvent<EntityRepositionRequest>(OnEntityRepositionRequested);
        }

        public override void Dispose()
        {
            base.Dispose();
            _globalEventBus.Unsubscribe<EntityDiedEvent>(OnEntityDied);
        }

        protected override void OnEntityAdded(Entity entity)
        {
            base.OnEntityAdded(entity);

            entity.SubscribeOnce<EntityBoundEvent>(e =>
            {
                if (entity.TryGetModule<WanderBehaviourModule>(out var wanderModule))
                    wanderModule.AnchorPoint = entity.GetModule<SpatialModule>().Position;
            });
        }

        public void Tick(float delta)
        {
            IterateMatchingEntities(entity => TickEntity(entity, delta));
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

            OptimizedOperationUtils.ClearTillNull(controlModule.RawControlProviders);
            OptimizedOperationUtils.ClearTillNull(controlModule.UniqueControlProviders);

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

        private void OnEntityDied(EntityDiedEvent e)
        {
            if (!e.Victim.TryGetModule<IntentModule>(out var intent)) return;

            intent.SetMove(new(Vector2.zero));
        }

        private void OnStatusEffectChanged(StatusEffectChangedEvent e)
        {
            if (e.Kind == StatusEffectChangeKind.Added && e.StatusEffect.Definition.ControlProvider != null)
            {
                var controlModule = e.Entity.GetModule<ControlModule>();
                controlModule.AddProvider(e.StatusEffect.Definition.ControlProvider.CreateInstance(_resolver));
            }
        }

        private void OnEntityRepositionRequested(EntityRepositionRequest request)
        {
            var controlModule = request.Entity.GetModule<ControlModule>();
            controlModule.ResetProviders();
        }
    }
}
