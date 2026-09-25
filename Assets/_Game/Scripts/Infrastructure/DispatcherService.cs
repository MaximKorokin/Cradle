using System;
using UnityEngine;
using VContainer.Unity;

namespace Assets._Game.Scripts.Infrastructure
{
    public sealed class DispatcherService : ITickable, ILateTickable, IFixedTickable
    {
        public event Action<float> OnTick;
        public event Action<float> OnLateTick;
        public event Action<float> OnFixedTick;

        public void FixedTick()
        {
            OnFixedTick?.Invoke(Time.fixedDeltaTime);
        }

        public void Tick()
        {
            OnTick?.Invoke(Time.deltaTime);
        }

        public void LateTick()
        {
            OnLateTick?.Invoke(Time.deltaTime);
        }
    }
}
