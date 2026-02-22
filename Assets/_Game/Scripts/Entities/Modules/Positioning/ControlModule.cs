using Assets._Game.Scripts.Entities.Control;
using Assets._Game.Scripts.Infrastructure;
using System;
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
            base.OnAttach();
            Subscribe<OverrideControlRequestEvent>(OnOverrideControlRequested);
        }

        private void OnOverrideControlRequested(OverrideControlRequestEvent overrideControlEvent)
        {

        }

        public void AddProvider(IControlProvider p)
        {
            _providers.Add(p);
        }

        public void RemoveProvider(IControlProvider p)
        {
            _providers.Remove(p);
        }

        public void Tick(float delta)
        {
            IControlProvider topPriorityProvider = null;
            var topPriority = int.MinValue;

            for (int i = 0; i < _providers.Count; i++)
            {
                var provider = _providers[i];
                if (!provider.IsActive) continue;

                var priority = (int)provider.Priority;
                if (priority > topPriority)
                {
                    topPriority = priority; 
                    topPriorityProvider = provider;
                }
            }

            topPriorityProvider?.Tick(Entity, delta);
        }

        public override void Dispose()
        {
            base.Dispose();

            _dispatcherService.OnTick -= Tick;
        }
    }

    public readonly struct OverrideControlRequestEvent : IEntityEvent
    {
        public readonly Entity Source;
        public readonly Entity Target;
        public readonly float Duration;

        public OverrideControlRequestEvent(Entity source, Entity target, float duration)
        {
            Source = source;
            Target = target;
            Duration = duration;
        }
    }
}
