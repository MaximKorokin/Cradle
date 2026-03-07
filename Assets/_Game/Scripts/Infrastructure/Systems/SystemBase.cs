using System;
using VContainer.Unity;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public abstract class SystemBase : IDisposable, IStartable
    {
        private readonly DispatcherService _dispatcher;

        protected SystemBase(DispatcherService dispatcher)
        {
            _dispatcher = dispatcher;
            _dispatcher.OnTick += OnTick;
            _dispatcher.OnFixedTick += OnFixedTick;
        }

        public virtual void Dispose()
        {
            _dispatcher.OnTick -= OnTick;
            _dispatcher.OnFixedTick -= OnFixedTick;
        }

        protected virtual void OnTick(float delta)
        {
        }

        protected virtual void OnFixedTick(float delta)
        {
        }

        public virtual void Start()
        {
        }
    }
}
