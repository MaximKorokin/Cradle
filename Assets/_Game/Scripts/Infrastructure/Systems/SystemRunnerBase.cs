using System;
using System.Collections.Generic;
using System.Linq;
using VContainer.Unity;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public abstract class SystemRunnerBase : IStartable, IDisposable
    {
        private readonly DispatcherService _dispatcherService;
        private readonly IReadOnlyList<IStartSystem> _startSystems;
        private readonly IReadOnlyList<ITickSystem> _tickSystems;
        private readonly IReadOnlyList<IFixedTickSystem> _fixedTickSystems;

        public SystemRunnerBase(
            DispatcherService dispatcherService,
            IReadOnlyList<ISystem> systems)
        {
            _dispatcherService = dispatcherService;
            _startSystems = systems.OfType<IStartSystem>().ToArray();
            _tickSystems = systems.OfType<ITickSystem>().ToArray();
            _fixedTickSystems = systems.OfType<IFixedTickSystem>().ToArray();
        }

        public void Start()
        {
            _dispatcherService.OnTick += OnTick;
            _dispatcherService.OnFixedTick += OnFixedTick;

            for (var i = 0; i < _startSystems.Count; i++)
            {
                _startSystems[i].Start();
            }
        }

        public void Dispose()
        {
            _dispatcherService.OnTick -= OnTick;
            _dispatcherService.OnFixedTick -= OnFixedTick;
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
