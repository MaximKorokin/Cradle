using Assets._Game.Scripts.Entities.Interactions;
using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class InteractionSystem : SystemBase
    {
        private readonly IGlobalEventBus _globalBus;
        private readonly DispatcherService _dispatcher;
        private readonly IObjectResolver _resolver;

        private readonly List<InteractionInstance> _active = new();
        private readonly IDisposable _subscription;

        public InteractionSystem(IGlobalEventBus globalBus, DispatcherService dispatcher, IObjectResolver resolver)
        {
            _globalBus = globalBus;
            _dispatcher = dispatcher;
            _resolver = resolver;

            _subscription = _globalBus.Subscribe<InteractionRequestEvent>(OnRequested);
            _dispatcher.OnTick += Tick;
        }

        private void OnRequested(InteractionRequestEvent e)
        {
            var root = e.Definition.BuildRuntime(_resolver);

            var ctx = new InteractionContext(
                e.Source,
                e.Target,
                e.Point,
                _globalBus);

            _active.Add(new InteractionInstance(root, ctx));
        }

        public void Tick(float delta)
        {
            for (int i = _active.Count - 1; i >= 0; i--)
            {
                if (_active[i].Tick(delta))
                    _active.RemoveAt(i);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            _subscription.Dispose();
            _dispatcher.OnTick -= Tick;
        }
    }

    public readonly struct InteractionRequestEvent : IGlobalEvent
    {
        public readonly InteractionDefinition Definition;
        public readonly Entity Source;
        public readonly Entity Target;
        public readonly Vector2 Point;

        public InteractionRequestEvent(
            InteractionDefinition definition,
            Entity source,
            Entity target,
            Vector2 point)
        {
            Definition = definition;
            Source = source;
            Target = target;
            Point = point;
        }
    }
}