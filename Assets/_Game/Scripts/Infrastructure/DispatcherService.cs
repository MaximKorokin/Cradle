using System;
using UnityEngine;
using VContainer.Unity;

namespace Assets._Game.Scripts.Infrastructure
{
    public sealed class DispatcherService : ITickable, IFixedTickable
    {
        public event Action<float> OnTick;
        public event Action<float> OnFixedTick;

        public void FixedTick()
        {
            OnFixedTick?.Invoke(Time.fixedDeltaTime);
        }

        public void Tick()
        {
            OnTick?.Invoke(Time.deltaTime);
        }
    }
}
