using Assets._Game.Scripts.Infrastructure;
using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets._Game.Scripts.Entities.Interactions
{
    public sealed class InteractionSystem : IDisposable, IStartable
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

            _subscription = _globalBus.Subscribe<InteractionRequestedEvent>(OnRequested);
            _dispatcher.OnTick += Tick;
        }

        private void OnRequested(InteractionRequestedEvent e)
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

        public void Dispose()
        {
            _subscription.Dispose();
            _dispatcher.OnTick -= Tick;
        }

        public void Start()
        {

        }
    }

    public readonly struct InteractionRequestedEvent : IGlobalEvent
    {
        public readonly InteractionDefinition Definition;
        public readonly Entity Source;
        public readonly Entity Target;
        public readonly Vector2 Point;

        public InteractionRequestedEvent(
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
