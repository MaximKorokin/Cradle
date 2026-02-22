using System;
using UnityEngine;
using VContainer.Unity;

namespace Assets._Game.Scripts.Infrastructure
{
    public sealed class DispatcherService : ITickable
    {
        public event Action<float> OnTick;

        public void Tick()
        {
            OnTick?.Invoke(Time.deltaTime);
        }
    }
}
