using Assets._Game.Scripts.Entities.Control;
using Assets._Game.Scripts.Infrastructure;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class ControlModule : EntityModuleBase
    {
        private readonly DispatcherService _dispatcherService;
        private readonly List<IControlProvider> _providers = new();

        public ControlModule(DispatcherService dispatcherService)
        {
            _dispatcherService = dispatcherService;
            _dispatcherService.OnTick += Tick;
        }

        protected override void OnAttach()
        {
            foreach (var provider in _providers)
            {
                provider.Initialize(Entity);
            }

            Subscribe<OverrideControlRequestEvent>(OnOverrideControlRequested);
        }

        private void OnOverrideControlRequested(OverrideControlRequestEvent overrideControlEvent)
        {
            AddProvider(overrideControlEvent.ControlProvider);
        }

        public void AddProvider(IControlProvider p)
        {
            if (Entity != null)
            {
                p.Initialize(Entity);
            }

            _providers.Add(p);
        }

        public void RemoveProvider(IControlProvider p)
        {
            _providers.Remove(p);
        }

        public void Tick(float delta)
        {
            for (int i = 0; i < _providers.Count; i++)
            {
                if (!_providers[i].IsActive)
                {
                    _providers.RemoveAt(i);
                }
            }

            var moveProvider = SelectBest(ControlMask.Move);
            var aimProvider = SelectBest(ControlMask.Aim);
            var interactProvider = SelectBest(ControlMask.Interact);

            moveProvider?.Tick(delta);
            aimProvider?.Tick(delta);
            interactProvider?.Tick(delta);
        }

        private IControlProvider SelectBest(ControlMask channel)
        {
            IControlProvider best = null;
            int bestPriority = int.MinValue;

            for (int i = 0; i < _providers.Count; i++)
            {
                var provider = _providers[i];
                if (!provider.IsActive) continue;
                if ((provider.Mask & channel) == 0) continue;

                var priority = (int)provider.Priority;
                if (priority > bestPriority || priority == bestPriority)
                {
                    best = provider;
                    bestPriority = priority;
                }
            }

            return best;
        }

        public override void Dispose()
        {
            base.Dispose();

            _dispatcherService.OnTick -= Tick;
        }
    }

    public readonly struct OverrideControlRequestEvent : IEntityEvent
    {
        public readonly IControlProvider ControlProvider;

        public Entity Entity { get; }

        public OverrideControlRequestEvent(Entity entity, IControlProvider controlProvider)
        {
            Entity = entity;
            ControlProvider = controlProvider;
        }
    }
}
