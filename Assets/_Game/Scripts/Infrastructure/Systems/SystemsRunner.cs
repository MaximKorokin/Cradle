using System;
using System.Collections.Generic;
using VContainer.Unity;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class SystemsRunner : IDisposable, IStartable
    {
        private readonly DispatcherService _dispatcherService;
        private readonly IReadOnlyList<ITickSystem> _tickSystems;
        private readonly IReadOnlyList<IFixedTickSystem> _fixedTickSystems;

        public SystemsRunner(
            DispatcherService dispatcherService,
            IReadOnlyList<ISystem> systems,
            IReadOnlyList<ITickSystem> tickSystems,
            IReadOnlyList<IFixedTickSystem> fixedTickSystems)
        {
            _dispatcherService = dispatcherService;
            _tickSystems = tickSystems;
            _fixedTickSystems = fixedTickSystems;

            _dispatcherService.OnTick += OnTick;
            _dispatcherService.OnFixedTick += OnFixedTick;
        }

        private void OnTick(float delta)
        {
            for (var i = 0; i < _tickSystems.Count; i++)
            {
                _tickSystems[i].Tick(delta);
            }
        }

        private void OnFixedTick(float delta)
        {
            for (var i = 0; i < _fixedTickSystems.Count; i++)
            {
                _fixedTickSystems[i].FixedTick(delta);
            }
        }

        public void Dispose()
        {
            _dispatcherService.OnTick -= OnTick;
            _dispatcherService.OnFixedTick -= OnFixedTick;
        }

        public void Start()
        {
        }
    }

    public interface ITickSystem
    {
        void Tick(float delta);
    }

    public interface IFixedTickSystem
    {
        void FixedTick(float delta);
    }
}
