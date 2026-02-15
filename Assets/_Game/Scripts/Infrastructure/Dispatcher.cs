using System;
using VContainer.Unity;

namespace Assets._Game.Scripts.Infrastructure
{
    public sealed class Dispatcher : ITickable
    {
        public event Action OnTick;

        public void Tick()
        {
            OnTick?.Invoke();
        }
    }
}
