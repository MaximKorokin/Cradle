using System;
using System.Collections.Generic;
using VContainer.Unity;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class SystemsRunner : IDisposable, IStartable
    {
        private readonly DispatcherService _dispatcherService;
        private readonly IReadOnlyList<IStartSystem> _startSystems;
        private readonly IReadOnlyList<ITickSystem> _tickSystems;
        private readonly IReadOnlyList<IFixedTickSystem> _fixedTickSystems;

        public SystemsRunner(
            DispatcherService dispatcherService,
            IReadOnlyList<ISystem> systems, // for auto initialization of systems in the container
            IReadOnlyList<IStartSystem> startSystems,
            IReadOnlyList<ITickSystem> tickSystems,
            IReadOnlyList<IFixedTickSystem> fixedTickSystems)
        {
            _dispatcherService = dispatcherService;
            _startSystems = startSystems;
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
            for (var i = 0; i < _startSystems.Count; i++)
            {
                _startSystems[i].Start();
            }
        }
    }

    public interface IStartSystem
    {
        void Start();
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
